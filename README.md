# Prompt Object Generation Layer (POGLayer)

This Unity project is a VR experiment that enables the generation of 3D models directly via voice prompts. In practice, the user speaks the name of the item they wish to spawn; the system generates the corresponding model, and the object becomes immediately manipulable within the virtual environment. The goal is to make materialization and spatial interaction far more intuitive, eliminating the need for cumbersome interfaces. Within the editor, the generated elements are stored in the `Assets/Models` folder.

Regarding the setup of the development environment, this repository's structure relies on Git submodules. This means that a standard archive download or a basic clone will not retrieve all the code required for the project to run correctly.

To obtain all the files right from the start, you need to use the command line to clone the project while including its dependencies. Navigate to your desired destination folder and run the following command:

```bash
git clone --recursive https://github.com/Henristote/PrompObjectGenerationLayerPOGLayer.git
```

If you have already performed a standard clone out of habit, the oversight is easily rectified. Open your terminal, navigate to the root of the project you just downloaded, and run the command to update the submodules:

```bash
git submodule update --init --recursive
```

This action will inspect the directory structure, initialize missing links, and download the required components.

Once all the files are on your machine, simply add the folder via Unity Hub and open the project. You will then be able to test the entire pipeline, from voice capture to physically manipulating the generated 3D object within your scene.
