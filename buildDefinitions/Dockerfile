FROM rockylinux:9

ARG LICENSE_SERVER

ENV TZ=America/Los_Angeles \
    SHELL=/bin/bash \
    LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN set -eux; \
    dnf -y --refresh update; \
    dnf -y install dnf-plugins-core ca-certificates tar gzip git jq unzip; \
    dnf -y config-manager --set-enabled crb || true; \
    # .NET SDK 10 (Rocky 9 AppStream)
    dnf -y install dotnet-sdk-10.0; \
    dnf clean all; \
    rm -rf /var/cache/dnf

# --- Synergy/DE Linux (x64) runtime --------------------------------------------
# Build context is staged by the pipeline. Requires BuildKit for the bind mount.
COPY ./linux.auto.install /opt/synergex/linux.auto.install
RUN --mount=type=bind,source=sde-archive.a,target=/tmp/sde/sde-archive.a \
    set -ex; \
    test -n "$LICENSE_SERVER"; \
    dnf -y install cpio libnsl ncurses-libs; \
    mkdir -p /azp; \
    cd /opt/synergex; \
    cp /tmp/sde/sde-archive.a ./428SDE.a; \
    sed -i 's/\r$//' linux.auto.install; \
    chmod +x linux.auto.install; \
    ./linux.auto.install -n "$LICENSE_SERVER" -f -c; \
    rm -f 428SDE.a; \
    . /opt/synergex/synergyde/setsde; \
    lmu -nf "$LICENSE_SERVER"; \
    printf '. /opt/synergex/synergyde/setsde\n' > /azp/setsde
# ------------------------------------------------------------------------------

RUN dotnet --info && git --version && jq --version

ENV TARGETARCH="linux-x64"
WORKDIR /azp
COPY ./startagent.sh ./
RUN chmod +x ./startagent.sh

ENV AGENT_ALLOW_RUNASROOT="true"

ENTRYPOINT ./startagent.sh
