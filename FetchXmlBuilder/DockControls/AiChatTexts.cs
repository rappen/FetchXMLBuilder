using Rappen.AI.WinForm;
using Rappen.XTB.FXB.Settings;
using Rappen.XTB.Helpers;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    internal sealed class AiChatTexts
    {
        private readonly AiProvider provider;
        private readonly AiModel model;
        private readonly string localFolder;
        private readonly Strictness strictness;

        public AiChatTexts(AiProvider provider, AiModel model, string localFolder, Strictness strictness)
        {
            this.provider = provider;
            this.model = model;
            this.localFolder = localFolder;
            this.strictness = strictness;
        }

        public string System => LoadPrompt(
            model?.Prompts?.System,
            provider?.Prompts?.System,
            OnlineSettings.Instance.AiSupport.PromptsV2.System);

        public string Behavior => LoadPrompt(
            model?.Prompts?.Behavior,
            provider?.Prompts?.Behavior,
            OnlineSettings.Instance.AiSupport.PromptsV2.Behavior);

        public string Style => LoadPrompt(
            model?.Prompts?.Style,
            provider?.Prompts?.Style,
            OnlineSettings.Instance.AiSupport.PromptsV2.Style);

        public string Preferences => LoadPrompt(
            model?.Prompts?.Preferences,
            provider?.Prompts?.Preferences,
            OnlineSettings.Instance.AiSupport.PromptsV2.Preferences);

        public string Strictness => OnlineFile.GetTextFromMaybeUrl(
            (model?.Prompts?.Strictness ??
             provider?.Prompts?.Strictness ??
             OnlineSettings.Instance.AiSupport.PromptsV2.Strictness)
            .Replace("{{strictness}}", strictness.ToString()),
            localFolder).Trim();

        public string UserFlavors => LoadPrompt(
            model?.Prompts?.UserFlavors,
            provider?.Prompts?.UserFlavors,
            OnlineSettings.Instance.AiSupport.PromptsV2.UserFlavors);

        public string UpdatedQuery => LoadPrompt(
            model?.Prompts?.Updated,
            provider?.Prompts?.Updated,
            OnlineSettings.Instance.AiSupport.PromptsV2.Updated);

        public string EntityMeta => LoadPrompt(
            model?.Prompts?.EntityMeta,
            provider?.Prompts?.EntityMeta,
            OnlineSettings.Instance.AiSupport.PromptsV2.EntityMeta);

        public string AttributeMeta => LoadPrompt(
            model?.Prompts?.AttributeMeta,
            provider?.Prompts?.AttributeMeta,
            OnlineSettings.Instance.AiSupport.PromptsV2.AttributeMeta);

        public string RelationshipMeta => LoadPrompt(
            model?.Prompts?.RelationshipMeta,
            provider?.Prompts?.RelationshipMeta,
            OnlineSettings.Instance.AiSupport.PromptsV2.RelationshipMeta);

        public string ToolRunQuery => LoadText(OnlineSettings.Instance.AiSupport.Tools.DescExecuteQuery);
        public string ToolUpdateQuery => LoadText(OnlineSettings.Instance.AiSupport.Tools.DescUpdateQuery);
        public string ToolMatchTable => LoadText(OnlineSettings.Instance.AiSupport.Tools.DescMatchTable);
        public string ToolMatchRelationship => LoadText(OnlineSettings.Instance.AiSupport.Tools.DescMatchRelationship);
        public string ToolMatchColumn => LoadText(OnlineSettings.Instance.AiSupport.Tools.DescMatchColumn);

        private string LoadPrompt(string modelValue, string providerValue, string fallbackValue) => LoadText(modelValue ?? providerValue ?? fallbackValue);

        private string LoadText(string value) => OnlineFile.GetTextFromMaybeUrl(value, localFolder).Trim();
    }
}