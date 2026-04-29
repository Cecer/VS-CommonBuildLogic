using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

#pragma warning disable IL3050, IL2026

internal class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 6)
        {
            Console.Error.WriteLine("Usage: UpdateModInfo <jsonPath> <version> <title> <description> <authors> <url>");
            return 1;
        }

        var jsonFile = args[0];
        var version = args[1];
        var title = args[2];
        var description = args[3];
        var authors = args[4];
        var url = args[5];


        string json;
        try
        {
            json = File.ReadAllText(jsonFile);
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine($"JSON file not found: {jsonFile}");
            return 2;
        }

        JsonObject jsonObject;
        try
        {
            jsonObject = JsonNode.Parse(json)!.AsObject();
        }
        catch
        {
            Console.Error.WriteLine($"Failed to parse modinfo.json file: {jsonFile}");
            return 3;
        }

        jsonObject["version"] = version;
        jsonObject["name"] = title;
        jsonObject["description"] = description;
        jsonObject["authors"] = new JsonArray(authors
            .Split(',')
            .Select(a => a.Trim())
            .Where(a => !string.IsNullOrEmpty(a))
            .Select(a => JsonValue.Create(a))
            .ToArray());
        jsonObject["website"] = url;

        var updatedJson = jsonObject.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(jsonFile, updatedJson);

        return 0;
    }
}