using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestClient
{
    void SetAuthToken(string authToken);
    void PostRequest();
    void AuthorizedPostRequest();
}