
using UnityEditor;
using UnityEngine;

namespace TP.Editor {

    [CustomEditor(typeof(Global_CameraCrop))]
    public class Editor_CameraCropButton : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            Global_CameraCrop cameraCrop = (Global_CameraCrop)target;
            if (GUILayout.Button("Camera Crop")) {
                cameraCrop.UpdateCrop(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
            }
        }
    }
}
