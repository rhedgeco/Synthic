use std::fs;

use glob::glob;

fn main() {
    fs::create_dir_all("bindings").unwrap();

    let mut builder = csbindgen::Builder::default();
    for entry in glob("src/**/*.rs").expect("Failed to read glob pattern") {
        if let Ok(path) = entry {
            builder = builder.input_extern_file(path);
        }
    }

    builder
        .csharp_dll_name("synthic")
        .csharp_namespace("Synthic.Native")
        .csharp_class_name("Lib")
        .csharp_class_accessibility("internal")
        .csharp_dll_name_if("UNITY_IOS && !UNITY_EDITOR", "__Internal")
        .generate_csharp_file("bindings/SynthicNative.cs")
        .unwrap();
}
