using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class PostRequest
{
    private Dictionary<string, string> values = new Dictionary<string, string>();

    public void AddValue<T>(string key, T value)
    {
        values.Add(key, value.ToString());
    }

    public async Task<HttpResponseMessage> MakeRequestAsync()
    {
        var client = new HttpClient();

        var values = new Dictionary<string, string>
        {
            {"thing1", "hello"},
            {"thing2", "world"}
        };

        var content = new FormUrlEncodedContent(values);

        return await client.PostAsync("http://www.example.com/recepticle.aspx", content);
    }
}