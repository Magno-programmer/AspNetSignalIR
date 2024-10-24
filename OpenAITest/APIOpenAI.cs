using OpenAI_API;

namespace AspNetSignalIR.OpenAITest
{
    public class APIOpenAI
    {
        public static string Conectar(string texto)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY");
            var client = new OpenAIAPI(apiKey);

            var chat = client.Chat.CreateConversation();

            chat.AppendSystemMessage($"Com resposta muito curta, responda: {texto}");

            string conection = chat.GetResponseFromChatbotAsync().GetAwaiter().GetResult();

            return conection;
        }
    }
}
