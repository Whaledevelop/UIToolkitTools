#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

public static class RecorderUtility
{
    public static void CaptureScreenshot(string outputPath)
    {
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        var controller = new RecorderController(controllerSettings);

        var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
        imageRecorder.name = "ScreenshotRecorder";
        imageRecorder.Enabled = true;
        imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
        imageRecorder.CaptureAlpha = true;
        imageRecorder.OutputFile = outputPath;
        imageRecorder.imageInputSettings = new GameViewInputSettings();

        controllerSettings.AddRecorderSettings(imageRecorder);
        controllerSettings.SetRecordModeToSingleFrame(0);
        controllerSettings.FrameRate = 60;

        controller.PrepareRecording();
        controller.StartRecording();
    }
}
#endif