#!/usr/bin/env bash
# Build + upload the site to Hostinger in one step.
#
#   ALSMOW_PWFILE=/path/to/ssh-password.txt ./deploy.sh            # staging (new.alsmow.com)
#   ALSMOW_PWFILE=... ALSMOW_DOCROOT=domains/alsmow.com/public_html ./deploy.sh   # production (later)
#
# Needs PuTTY (plink + pscp). The server keeps api/config.php and uploads/ untouched.
set -euo pipefail
cd "$(dirname "$0")"

HOST=147.93.92.68
PORT=65002
USER_=u920789781
HOSTKEY="SHA256:hJg0zax3LfUTlG9TMwDCFWtXxO1TPe7/T2NgMEv6OrA"
DOCROOT="${ALSMOW_DOCROOT:-domains/alsmow.com/public_html/new}"
: "${ALSMOW_PWFILE:?set ALSMOW_PWFILE to a file containing the SSH password}"

SSH=(plink -batch -ssh -P "$PORT" -hostkey "$HOSTKEY" -pwfile "$ALSMOW_PWFILE" "$USER_@$HOST")
SCP=(pscp -batch -P "$PORT" -hostkey "$HOSTKEY" -pwfile "$ALSMOW_PWFILE")

echo "==> build"
(cd web && npm run build)

echo "==> package"
rm -rf .deploy && mkdir -p .deploy/pkg
cp -r web/dist/. .deploy/pkg/
mkdir -p .deploy/pkg/api
cp -r api/index.php api/.htaccess api/config.sample.php api/lib api/migrations api/tools .deploy/pkg/api/
cp deploy/uploads.htaccess .deploy/pkg/uploads.htaccess
tar -czf .deploy/release.tgz -C .deploy/pkg .

echo "==> upload"
"${SCP[@]}" .deploy/release.tgz "$USER_@$HOST:alsmow_release.tgz"

echo "==> install on server ($DOCROOT)"
"${SSH[@]}" bash -s <<EOF
set -e
cd ~/$DOCROOT
rm -rf assets api/lib api/migrations api/tools
tar -xzf ~/alsmow_release.tgz
rm -f ~/alsmow_release.tgz
mkdir -p uploads && mv -f uploads.htaccess uploads/.htaccess
for f in api/index.php api/lib/*.php api/tools/*.php; do php -l "\$f" >/dev/null || { echo "PHP syntax error in \$f"; exit 1; }; done
if [ -f api/config.php ]; then php api/tools/install.php migrate; else echo "!! api/config.php missing: create it before the API can work"; fi
echo "deployed: \$(date)"
EOF
echo "==> done"
