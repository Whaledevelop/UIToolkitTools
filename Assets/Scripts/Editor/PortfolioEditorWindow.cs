// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UIElements;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using System.Collections.Generic;
// using System.IO;
//
// public class PortfolioEditorWindow : OdinEditorWindow
// {
//     [MenuItem("Tools/Portfolio Editor")]
//     private static void OpenWindow()
//     {
//         GetWindow<PortfolioEditorWindow>("Portfolio Editor").Show();
//     }
//
//     [TabGroup("Personal Info")]
//     [BoxGroup("Personal Info/Basic Information")]
//     [LabelText("Full Name")]
//     public string fullName = "НИКИТА СЕРЕБРЯКОВ";
//
//     [BoxGroup("Personal Info/Basic Information")]
//     [LabelText("Position")]
//     public string position = "SENIOR UNITY DEVELOPER";
//
//     [BoxGroup("Personal Info/Basic Information")]
//     [LabelText("Profile Picture")]
//     [PreviewField(100)]
//     public Texture2D profilePicture;
//
//     [TabGroup("Content")]
//     [BoxGroup("Content/Left Column")]
//     [LabelText("Contact Information")]
//     [ListDrawerSettings(ShowFoldout = true)]
//     public List<string> contactInfo = new List<string>
//     {
//         "@yourusername",
//         "C#",
//         "DOTween",
//         "UI Toolkit"
//     };
//
//     [BoxGroup("Content/Left Column")]
//     [LabelText("Skills")]
//     [ListDrawerSettings(ShowFoldout = true)]
//     public List<string> skills = new List<string>
//     {
//         "yourusername",
//         "C#",
//         "DOTween",
//         "UI Toolkit"
//     };
//
//     [BoxGroup("Content/Left Column")]
//     [LabelText("Links")]
//     [ListDrawerSettings(ShowFoldout = true)]
//     public List<string> links = new List<string>
//     {
//         "github.com/yourusernam",
//         "github-comoursername"
//     };
//
//     [TabGroup("Content")]
//     [BoxGroup("Content/Right Column")]
//     [LabelText("About Me")]
//     [TextArea(3, 5)]
//     public string aboutMe = "Высокая 1.5 лет оачиание вермонечности и янейевые";
//
//     [Serializable]
//     public class ProjectExperience
//     {
//         [LabelText("Company/Project Name")]
//         public string companyName;
//         
//         [LabelText("Responsibilities")]
//         [ListDrawerSettings(ShowFoldout = true)]
//         public List<string> responsibilities = new List<string>();
//     }
//
//     [BoxGroup("Content/Right Column")]
//     [LabelText("Projects/Experience")]
//     [ListDrawerSettings(ShowFoldout = true)]
//     public List<ProjectExperience> projectExperience = new List<ProjectExperience>
//     {
//         new ProjectExperience
//         {
//             companyName = "TapEmpire - Созданиебори",
//             responsibilities = new List<string>
//             {
//                 "Ryper–casual games operaat",
//                 "Satisfying puzzles internal library and tomplate for new gemes"
//             }
//         },
//         new ProjectExperience
//         {
//             companyName = "Immortal Hunters",
//             responsibilities = new List<string>
//             {
//                 "Meta gameplay (city, quests, character progression, map",
//                 "Dialogue system node-basd editor Sounds, and some parts of core gameplay (combat)"
//             }
//         },
//         new ProjectExperience
//         {
//             companyName = "GGWP",
//             responsibilities = new List<string>
//             {
//                 "Developed browser fighting game",
//                 "Combat system, client-server connection with blockchain"
//             }
//         },
//         new ProjectExperience
//         {
//             companyName = "Playgendary",
//             responsibilities = new List<string>
//             {
//                 "R&D/Services Great Empire",
//                 "Internal advertisemen, purchases, analytics plugins, and integration"
//             }
//         },
//         new ProjectExperience
//         {
//             companyName = "MP Games",
//             responsibilities = new List<string>
//             {
//                 "Match 2 game Great Empire"
//             }
//         }
//     };
//
//     [TabGroup("Settings")]
//     [BoxGroup("Settings/UI Document")]
//     [LabelText("UI Document Asset")]
//     [FilePath(Extensions = "uxml")]
//     public string uiDocumentPath = "Assets/UIToolkit/Portfolio.uxml";
//
//     [BoxGroup("Settings/UI Document")]
//     [LabelText("USS File Path")]
//     [FilePath(Extensions = "uss")]
//     public string ussFilePath = "Assets/UIToolkit/Portfolio.uss";
//
//     [TabGroup("Preview")]
//     [BoxGroup("Preview/Actions")]
//     [Button("Generate Preview", ButtonSizes.Large)]
//     public void GeneratePreview()
//     {
//         // Create a temporary UI Document GameObject to preview the portfolio
//         GameObject previewObject = new GameObject("Portfolio Preview");
//         UIDocument uiDocument = previewObject.AddComponent<UIDocument>();
//         
//         // Get existing UXML asset
//         var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uiDocumentPath);
//         if (uxmlAsset == null)
//         {
//             Debug.LogError($"UXML file not found at path: {uiDocumentPath}");
//             DestroyImmediate(previewObject);
//             return;
//         }
//         
//         uiDocument.visualTreeAsset = uxmlAsset;
//         
//         // Update UI elements with current values
//         EditorApplication.delayCall += () =>
//         {
//             var root = uiDocument.rootVisualElement;
//             
//             // Update name and position
//             root.Q<Label>("name").text = fullName;
//             root.Q<Label>("position").text = position;
//             
//             // Update profile picture
//             if (profilePicture != null)
//             {
//                 var photoElement = root.Q<VisualElement>("photo");
//                 photoElement.style.backgroundImage = new StyleBackground(profilePicture);
//             }
//             
//             // Clear and update left column
//             var leftColumn = root.Q("leftColumn");
//             leftColumn.Clear();
//             
//             // Add contacts section
//             leftColumn.Add(new Label("КОНТАКТЫ") { classList = { "section-header" } });
//             foreach (var contact in contactInfo)
//             {
//                 leftColumn.Add(new Label($"• {contact}"));
//             }
//             
//             // Add skills section
//             leftColumn.Add(new Label("НАВЫКИ") { classList = { "section-header" } });
//             foreach (var skill in skills)
//             {
//                 leftColumn.Add(new Label($"• {skill}"));
//             }
//             
//             // Add links section
//             leftColumn.Add(new Label("НАВЫКИ") { classList = { "section-header" } });
//             foreach (var link in links)
//             {
//                 leftColumn.Add(new Label(link));
//             }
//             
//             // Clear and update right column
//             var rightColumn = root.Q("rightColumn");
//             rightColumn.Clear();
//             
//             // Add about me section
//             rightColumn.Add(new Label("ОБО МНЕ") { classList = { "section-header" } });
//             rightColumn.Add(new TextElement { text = aboutMe });
//             
//             // Add project experience
//             rightColumn.Add(new Label("Обо мне") { classList = { "section-header", "section-margin-top" } });
//             
//             foreach (var project in projectExperience)
//             {
//                 rightColumn.Add(new Label(project.companyName) { classList = { "company-name", "section-margin-top" } });
//                 
//                 foreach (var responsibility in project.responsibilities)
//                 {
//                     rightColumn.Add(new Label($"• {responsibility}"));
//                 }
//             }
//             
//             Debug.Log("Portfolio preview generated successfully!");
//         };
//     }
//
//     [BoxGroup("Preview/Actions")]
//     [Button("Save Changes to UXML", ButtonSizes.Large)]
//     public void SaveChangesToUXML()
//     {
//         // This is a simplified implementation - in a real scenario, you'd want to:
//         // 1. Parse the UXML file
//         // 2. Update the relevant elements
//         // 3. Write it back to the file
//         
//         EditorUtility.DisplayDialog("Save Changes", 
//             "This would save your changes to the UXML file.\n\nIn a complete implementation, this would update the actual UXML file with your changes.", 
//             "OK");
//         
//         // For a real implementation, you'd use something like:
//         // var serializer = new XmlSerializer(typeof(YourUXMLDataType));
//         // using (var writer = new StreamWriter(uiDocumentPath))
//         // {
//         //     serializer.Serialize(writer, yourUXMLData);
//         // }
//     }
//
//     [BoxGroup("Preview/Actions")]
//     [Button("Generate Runtime Prefab", ButtonSizes.Large)]
//     public void GenerateRuntimePrefab()
//     {
//         // Create a new GameObject with UI Document
//         GameObject portfolioObject = new GameObject("Portfolio UI");
//         UIDocument uiDocument = portfolioObject.AddComponent<UIDocument>();
//         
//         // Set the UXML asset
//         var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uiDocumentPath);
//         if (uxmlAsset == null)
//         {
//             Debug.LogError($"UXML file not found at path: {uiDocumentPath}");
//             DestroyImmediate(portfolioObject);
//             return;
//         }
//         
//         uiDocument.visualTreeAsset = uxmlAsset;
//         
//         // Add a script to handle runtime updates
//         PortfolioManager manager = portfolioObject.AddComponent<PortfolioManager>();
//         
//         // Set the profile picture
//         var serializedObject = new SerializedObject(manager);
//         var avatarProperty = serializedObject.FindProperty("avatarTexture");
//         avatarProperty.objectReferenceValue = profilePicture;
//         serializedObject.ApplyModifiedProperties();
//         
//         // Create the prefab
//         string prefabPath = "Assets/Prefabs";
//         if (!Directory.Exists(prefabPath))
//         {
//             Directory.CreateDirectory(prefabPath);
//         }
//         
//         string fullPath = $"{prefabPath}/Portfolio.prefab";
//         bool success = false;
//         
//         PrefabUtility.SaveAsPrefabAsset(portfolioObject, fullPath, out success);
//         DestroyImmediate(portfolioObject);
//         
//         if (success)
//         {
//             Debug.Log($"Portfolio prefab created successfully at: {fullPath}");
//             EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<GameObject>(fullPath));
//         }
//         else
//         {
//             Debug.LogError("Failed to create portfolio prefab");
//         }
//     }
// }