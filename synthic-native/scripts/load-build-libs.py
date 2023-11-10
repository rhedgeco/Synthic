import shutil
import pathlib
import sys

script_dir = pathlib.Path(__file__).parent.resolve()


def main():
    # get linux library src
    linux_lib_src_path = script_dir.parent.joinpath(
        "target", "x86_64-unknown-linux-gnu", "release", "libsynthic.so"
    )
    if not linux_lib_src_path.exists():
        print(f"library '{linux_lib_src_path}' does not exist", file=sys.stderr)
        sys.exit(1)

    # get windows library src
    windows_lib_src_path = script_dir.parent.joinpath(
        "target", "x86_64-pc-windows-gnu", "release", "synthic.dll"
    )
    if not windows_lib_src_path.exists():
        print(f"library '{windows_lib_src_path}' does not exist", file=sys.stderr)
        sys.exit(1)

    # get binding file src
    cs_src_path = script_dir.parent.joinpath("bindings", "SynthicNative.cs")
    if not cs_src_path.exists():
        print(f"binding '{cs_src_path}' does not exist", file=sys.stderr)
        sys.exit(1)

    # create dst paths
    native_dir = script_dir.parent.parent.joinpath(
        "Assets",
        "Synthic",
        "Scripts",
        "Native",
    )
    plugins_dir = native_dir.joinpath("Plugins")
    linux_lib_dst_path = plugins_dir.joinpath("x86_64", "libsynthic.so")
    windows_lib_dst_path = plugins_dir.joinpath("x86_64", "synthic.dll")
    cs_dst_path = native_dir.joinpath("SynthicNative.cs")

    # remove all current plugins if they exist
    if plugins_dir.exists():
        shutil.rmtree(plugins_dir)

    # create necessary directories
    linux_lib_dst_path.parent.mkdir(parents=True, exist_ok=True)
    windows_lib_dst_path.parent.mkdir(parents=True, exist_ok=True)
    cs_dst_path.parent.mkdir(parents=True, exist_ok=True)

    # copy all library and binding files
    shutil.copyfile(linux_lib_src_path, linux_lib_dst_path)
    shutil.copyfile(windows_lib_src_path, windows_lib_dst_path)
    shutil.copyfile(cs_src_path, cs_dst_path)


if __name__ == "__main__":
    main()
