@echo off
pushd %~dp0

git submodule update --init --recursive

rem Fast-forward each submodule to its remote
git submodule foreach "git fetch && git checkout master && git pull --ff-only"

popd