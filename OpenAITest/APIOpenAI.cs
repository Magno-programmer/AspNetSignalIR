using OpenAI_API;

namespace AspNetSignalIR.OpenAITest
{
    public class APIOpenAI
    {
        public async static Task<string> Conectar(string texto)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("apiKey does not exist");
                return "API key not set.";
            }

            var apiAuth = new APIAuthentication(apiKey);
            var client = new OpenAIAPI(apiAuth);

            var chat = client.Chat.CreateConversation();

            chat.AppendSystemMessage($"Com resposta muito curta, responda: {texto}");

            return await chat.GetResponseFromChatbotAsync();

        }
    }
}
