#!/bin/bash
# Watch-free build for Emperors and Consuls (RimWorld 1.6 first).

MOD="EmperorsAndConsuls"
solutionPath="Source/${MOD}.sln"
configurations=("v1.6")

dotnet restore "$solutionPath"

function sync_mod() {
    rsync -a --exclude 'Source' --exclude '.git' --exclude 'obj' --exclude 'bin' \
        --exclude '*.sln' --exclude '*.csproj' \
        ./ "/rimworld/1.2/Mods/${MOD}/"

    rm -rf /rimworld/1.6/Mods/${MOD}
    rm -rf /rimworld/1.6-steam/Mods/${MOD}

    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.6/Mods
    cp -af /rimworld/1.2/Mods/${MOD} /rimworld/1.6-steam/Mods
}

function build() {
    local failed=0
    for config in "${configurations[@]}"; do
        echo "Building for configuration: $config"
        if ! dotnet build --no-restore "$solutionPath" --configuration "Release $config"; then
            echo "Build failed for $config"
            failed=1
        fi
    done

    if [[ $failed -eq 1 ]]; then
        echo "One or more builds failed. Aborting sync."
        return 1
    fi

    mkdir -p 1.6/Assemblies
    cp -f /rimworld/1.2/Mods/EmperorsAndConsuls/1.6/Assemblies/EmperorsAndConsuls.dll 1.6/Assemblies/ || true
    sync_mod
    echo "All builds completed!"
}

build || exit 1
echo "Done"
