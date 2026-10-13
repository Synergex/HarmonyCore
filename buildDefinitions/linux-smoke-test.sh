#!/bin/bash
#
# Smoke test for a Harmony Core Linux deployment.
#
# Starts a published harmonydemo deployment with the shell scripts the project template ships,
# exercises OData and a Traditional Bridge round trip over HTTPS, then stops it again. Covers the
# deployment scripts (startserver.sh, stopserver.sh, check.sh, launch.sh), the Synergy environment,
# and the bridge, none of which any other job in this pipeline runs.
#
# Used by the LinuxSmokeTest job, and runnable by hand against any deployment folder:
#
#   ./linux-smoke-test.sh                       # test the deployment in the current directory
#   ./linux-smoke-test.sh /path/to/deployment
#   EXPECT_HARMONY_VERSION=10.0.51 ./linux-smoke-test.sh
#
# Environment:
#   EXPECT_HARMONY_VERSION  when set, assert the deployment carries this Harmony.Core version
#   SETSDE                  path to the Synergy setsde script, if it is not already sourced
#   ENVIRONMENT_NAME        environment name to deploy as (default LinuxDemo)
#   HTTPS_PORT              port Kestrel is expected on (default 8086)

set -u

DEPLOY_DIR="${1:-$PWD}"
ENVIRONMENT_NAME="${ENVIRONMENT_NAME:-LinuxDemo}"
HTTPS_PORT="${HTTPS_PORT:-8086}"
SERVER="Services.Host.$ENVIRONMENT_NAME"
BASE="https://localhost:$HTTPS_PORT"

cd "$DEPLOY_DIR" || { echo "no such directory: $DEPLOY_DIR"; exit 1; }

# Report failures as pipeline errors when running under Azure Pipelines, plainly otherwise.
err() {
  if [ -n "${TF_BUILD:-}" ]; then echo "##vso[task.logissue type=error]$1"; else echo "ERROR: $1"; fi
}

cleanup() {
  pkill -f "$SERVER" 2> /dev/null
  pkill -f 'host\.dbr' 2> /dev/null
}

fail() {
  err "$1"
  echo "--- server log ---";  tail -n 200 "$SERVER"-*.log 2> /dev/null
  echo "--- deployment ---"; ls -la . SampleData 2> /dev/null | head -n 80
  echo "--- environment ---"; env | grep -E '^(DAT|DBLDIR|SDE|ASPNETCORE_)' | sort
  echo "--- bridge: what the Synergy runtime says about host.dbr ---"
  command -v dbs dbr 2> /dev/null
  ( timeout 30 ./launch.sh 6 < /dev/null 2>&1 | head -n 30 ) || true
  if ! pidof "$SERVER" > /dev/null && command -v strace > /dev/null; then
    # The host prints only the exception message, e.g. "File not found". Show which path it was.
    echo "--- files the service could not open (strace ENOENT) ---"
    ( . "./startserver.$ENVIRONMENT_NAME.config"; export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT_NAME"
      timeout 60 strace -f -qq -e trace=openat,open,stat,newfstatat,access -o strace.log "./$SERVER" > /dev/null 2>&1 || true )
    grep -h ENOENT strace.log 2> /dev/null | grep -o -E '"[^"]+"' |
      grep -v -E '^"/(proc|sys|usr|etc|lib|lib64|dev|tmp)/|\.so' | sort -u | tail -n 40
  fi
  cleanup
  exit 1
}

# -----------------------------------------------------------------------------------------------
echo "== 0. Synergy environment =="
if [ -z "${DBLDIR:-}" ]; then
  for candidate in "${SETSDE:-}" "${AGENT_HOMEDIRECTORY:-}/setsde" /azp/setsde /opt/synergex/synergyde/setsde; do
    [ -n "$candidate" ] && [ -f "$candidate" ] && { . "$candidate"; break; }
  done
fi
[ -n "${DBLDIR:-}" ] || fail "no Synergy environment; source setsde or set SETSDE"
echo "  DBLDIR=$DBLDIR"

# The deployment is assembled on Windows. Give it the line endings and permissions a real one gets.
if command -v dos2unix > /dev/null; then dos2unix -q ./*.sh ./*.config
else sed -i 's/\r$//' ./*.sh ./*.config; fi
chmod +x ./*.sh "./Services.Host"

if [ -n "${EXPECT_HARMONY_VERSION:-}" ]; then
  echo "== 0b. deployment must carry Harmony.Core $EXPECT_HARMONY_VERSION =="
  grep -q "\"Harmony.Core/$EXPECT_HARMONY_VERSION\"" Services.Host.deps.json ||
    fail "Services.Host.deps.json does not reference Harmony.Core $EXPECT_HARMONY_VERSION"
fi

echo "== 1. every script must parse =="
for f in ./*.sh; do bash -n "$f" || fail "$f does not parse"; done

echo "== 2. with no .environment, startserver.sh must refuse to start =="
rm -f .environment environment
out=$(./startserver.sh detach 2>&1); echo "$out"
echo "$out" | grep -q "hidden file '.environment'" ||
  fail "startserver.sh did not ask for the hidden file .environment"
pidof "$SERVER" > /dev/null && fail "the service started without an environment file"

echo "== 3. with .environment, startserver.sh detach =="
echo "$ENVIRONMENT_NAME" > .environment
./startserver.sh detach || true   # the shipped scripts exit 2 even on success (return/exit idiom)
sleep 3
pidof "$SERVER" > /dev/null || fail "$SERVER is not running after startserver.sh"

echo "== 4. wait for Kestrel on $BASE =="
code=000
for _ in $(seq 1 60); do
  code=$(curl -sk -o /dev/null -w '%{http_code}' "$BASE/\$metadata" || true)
  [ "$code" = "200" ] && break
  pidof "$SERVER" > /dev/null || fail "$SERVER exited during startup"
  sleep 2
done
[ "$code" = "200" ] || fail "GET /\$metadata returned $code after 120s"

echo "== 5. OData entity set =="
code=$(curl -sk -o /dev/null -w '%{http_code}' "$BASE/Customers" || true)
[ "$code" = "200" ] || fail "GET /Customers returned $code"

echo "== 6. Traditional Bridge round trip (Services.Host -> launch.sh -> dbs host.dbr) =="
code=$(curl -sk -o bridge.json -w '%{http_code}' -H 'Content-Type: application/json' \
       "$BASE/BridgeAPI/GetEnvironment" || true)
cat bridge.json 2> /dev/null; echo
[ "$code" = "200" ] || fail "GET /BridgeAPI/GetEnvironment returned $code"

echo "== 7. check.sh =="
./check.sh
./check.sh | grep -q "($SERVER): [0-9]" || fail "check.sh does not report the running service"

echo "== 8. stopserver.sh =="
./stopserver.sh || true
sleep 2
pidof "$SERVER" > /dev/null && fail "$SERVER is still running after stopserver.sh"

cleanup
echo "Linux smoke test OK: deployed, started, queried, bridged and stopped"
