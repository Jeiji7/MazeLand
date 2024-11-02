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
        // Находим все компоненты MonoBehaviour на сцене
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
                Debug.Log($"Добавлена команда: {method.Name}"); // Логирование добавленных команд
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
                    Debug.LogError($"Ошибка при выполнении команды: {ex}");
                    FindObjectOfType<ConsoleController>().LogMessage($"Ошибка при выполнении команды: {ex}");
                }
            }
        }
        else
        {
            Debug.LogError($"Команда '{command}' не найдена.");
            FindObjectOfType<ConsoleController>().LogMessage($"Команда '{command}' не найдена.");
        }
    }
}
