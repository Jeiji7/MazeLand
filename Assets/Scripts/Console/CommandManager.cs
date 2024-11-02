using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private Dictionary<string, MethodInfo> commandMethods = new Dictionary<string, MethodInfo>();
    private List<MonoBehaviour> commandTargets = new List<MonoBehaviour>();

    void Start()
    {
        // ������� ��� ���������� MonoBehaviour �� �����
        MonoBehaviour[] components = FindObjectsOfType<MonoBehaviour>();
        foreach (var component in components)
        {
            var methods = component.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0);

            foreach (var method in methods)
            {
                commandMethods[method.Name] = method;
                commandTargets.Add(component);
                Debug.Log($"��������� �������: {method.Name}"); // ����������� ����������� ������
            }
        }
    }

    public void ExecuteCommand(string command)
    {
        if (commandMethods.TryGetValue(command, out MethodInfo method))
        {
            int index = commandMethods.Keys.ToList().IndexOf(command);
            if (index >= 0 && index < commandTargets.Count)
            {
                MonoBehaviour target = commandTargets[index];
                try
                {
                    method.Invoke(target, null);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"������ ��� ���������� �������: {ex}");
                    FindObjectOfType<ConsoleController>().LogMessage($"������ ��� ���������� �������: {ex}");
                }
            }
        }
        else
        {
            Debug.LogError($"������� '{command}' �� �������.");
            FindObjectOfType<ConsoleController>().LogMessage($"������� '{command}' �� �������.");
        }
    }
}
