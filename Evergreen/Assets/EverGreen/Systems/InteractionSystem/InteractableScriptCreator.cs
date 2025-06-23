using UnityEditor;
using UnityEngine;
using System.IO;

public class InteractableScriptCreator : MonoBehaviour
{
   /* private string scriptName = "Interactable";

    private static InteractableScriptCreator windowInstance;

    [MenuItem("Tools/Create Interactable Script")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow<InteractableScriptCreator>("Create Interactable Script");
    }

    private void OnGUI()
    {
        GUILayout.Label("New Interactable Script", EditorStyles.boldLabel);

        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        if (GUILayout.Button("Create"))
        {
            CreateScript();
        }
    }

    private void CreateScript()
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            Debug.LogError("Script name cannot be empty.");
            return;
        }

        string folderPath = "Assets/EverGreen/Systems/InteractionSystem/InteractableItens";
        string filePath = Path.Combine(folderPath, scriptName + ".cs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            Debug.LogError("Script already exists at: " + filePath);
            return;
        }

        string scriptTemplate = $@"using UnityEngine;

public class {scriptName} : MonoBehaviour, IInteractable
{{
    [Header(""Item necessário para interação (opcional)"")]
    public Item requiredItem;
    public int requiredItemQuantity = 1;

    [TextArea]
    public string interactionMessage = ""Interagiu com Interactable"";

    // Implementação das propriedades da interface (nomes exatos conforme a interface)
    public Item RequiredItem => requiredItem;
    public int RequiredItemQuantity => requiredItemQuantity;
    public string InteractionMessage => interactionMessage;

    public void Interact()
    {{
        if (RequiredItem == null || InventoryManager.Instance.HasItem(RequiredItem, RequiredItemQuantity))
        {{
            PerformInteraction();
        }}
        else
        {{
            Debug.Log(""Item necessário: "" + RequiredItem.itemName);
        }}
    }}

    private void PerformInteraction()
    {{
        Debug.Log(InteractionMessage);
        // TODO: lógica específica de interação aqui.
    }}
}}";


        File.WriteAllText(filePath, scriptTemplate);
        AssetDatabase.Refresh();
        Debug.Log($"Script '{scriptName}.cs' criado em {folderPath}");

        // ✅ Fechar a janela após a criação
        if (windowInstance != null)
        {
            windowInstance.Close();
        }
        else
        {
            Close();
        }
    }*/
}
