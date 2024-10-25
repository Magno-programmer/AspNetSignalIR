using OpenAI_API;

namespace AspNetSignalIR.OpenAITest
{
    public class APIOpenAI
    {
        public async static Task<string> Conectar(string texto)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY");
            var client = new OpenAIAPI(apiKey);

            var chat = client.Chat.CreateConversation();

            chat.AppendSystemMessage($"Com resposta muito curta, responda: {texto}");

            return await chat.GetResponseFromChatbotAsync();

        }
    }
}
