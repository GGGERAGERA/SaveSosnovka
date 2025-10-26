using UnityEngine;
using System.Collections.Generic;
using System;

public static class SimpleJSON
{
    public static Dictionary<string, string> Parse(string json)
    {
        var dict = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(json)) return dict;

        // Убираем пробелы и переносы строк
        json = json.Replace("\r", "").Replace("\n", "").Replace(" ", "");

        // Ищем пары "ключ": "значение"
        int i = 0;
        while (i < json.Length)
        {
            if (json[i] == '"')
            {
                // Парсим ключ
                string key = ParseString(json, ref i);
                if (i >= json.Length || json[i] != ':') continue;
                i++; // пропускаем ':'

                // Парсим значение
                string value = "";
                if (json[i] == '"')
                    value = ParseString(json, ref i);
                else if (json[i] == '{' || json[i] == '[')
                {
                    // Пропускаем вложенные объекты/массивы (пока не нужны)
                    int depth = 1;
                    i++;
                    while (i < json.Length && depth > 0)
                    {
                        if (json[i] == '{' || json[i] == '[') depth++;
                        else if (json[i] == '}' || json[i] == ']') depth--;
                        i++;
                    }
                    value = ""; // пустая строка для вложенных объектов
                }
                else
                {
                    // Парсим число или true/false/null
                    int start = i;
                    while (i < json.Length && json[i] != ',' && json[i] != '}')
                    {
                        i++;
                    }
                    value = json.Substring(start, i - start).Trim();
                }

                // Добавляем в словарь
                if (!string.IsNullOrEmpty(key))
                    dict[key] = value;
            }
            i++;
        }

        return dict;
    }

    private static string ParseString(string json, ref int index)
    {
        index++; // пропускаем открывающую кавычку
        var sb = new System.Text.StringBuilder();
        while (index < json.Length)
        {
            char c = json[index++];
            if (c == '"') break; // закрывающая кавычка
            if (c == '\\')
            {
                if (index < json.Length)
                {
                    char n = json[index++];
                    switch (n)
                    {
                        case 't': sb.Append('\t'); break;
                        case 'r': sb.Append('\r'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case 'u':
                            if (index + 4 <= json.Length)
                            {
                                string hex = json.Substring(index, 4);
                                index += 4;
                                try
                                {
                                    sb.Append((char)Convert.ToInt32(hex, 16));
                                }
                                catch { sb.Append("\\u" + hex); }
                            }
                            else sb.Append("\\u");
                            break;
                        default: sb.Append('\\').Append(n); break;
                    }
                }
            }
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
}