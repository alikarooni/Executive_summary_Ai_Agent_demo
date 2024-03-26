using Newtonsoft.Json;
using System.Text;

namespace MS_Word_Creator;

public class AskChatGpt
{
    private readonly string _URL = @"http://127.0.0.1:62000/api/call_chatgpt";

    public AskChatGpt()
    {
    }

    public async Task<List<string>> Request(List<string> tables)
    {
        string jsonContent = JsonConvert.SerializeObject(tables);

        // Prepare the content for the POST request
        StringContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        using (var httpClient = new HttpClient())
        {
            // Make the POST request
            HttpResponseMessage response = await httpClient.PostAsync(_URL, httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Read the JSON response as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string back into a List of strings
                List<string> responseStrings = JsonConvert.DeserializeObject<List<string>>(jsonResponse);

                return responseStrings;
            }
        }

        return new List<string>();
    }
}