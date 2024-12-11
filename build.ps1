$OUTPUT_PATH = "./_Publish"
$INNO_SETUP_COMPILER = "C:/Program Files (x86)/Inno Setup 6/ISCC.exe"

Write-Output "Building Launcher...."
cd ./Source/Launcher
cargo build --release --target aarch64-pc-windows-msvc
cd ../..

ls -r Source/Launcher/target

Write-Output "Building Hurl...."
dotnet restore
dotnet publish -c Release -r win-arm64 --no-self-contained --output $OUTPUT_PATH ./Hurl.sln

Write-Output "Building Installer...."
& $INNO_SETUP_COMPILER ./Utils/installer.iss
