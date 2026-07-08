using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rappen.AI.WinForm;

namespace FXBTests
{
    /// <summary>
    /// Tests for <see cref="GitHubCopilotAuth"/> — the DPAPI secret roundtrip and the pure
    /// (network-free) parsing of the Copilot /models response. Network, token exchange and the
    /// WinForms settings wiring are covered by manual testing, not here.
    /// </summary>
    [TestClass]
    public class GitHubCopilotAuthTests
    {
        #region Protect / Unprotect (DPAPI)

        [TestMethod]
        public void ProtectUnprotect_Roundtrip_ReturnsOriginal()
        {
            var secret = "gho_ExampleToken_1234567890";
            var protectedText = GitHubCopilotAuth.Protect(secret);

            Assert.AreNotEqual(secret, protectedText, "Protected value should not equal the plain text.");
            Assert.AreEqual(secret, GitHubCopilotAuth.Unprotect(protectedText));
        }

        [TestMethod]
        public void Protect_EmptyOrNull_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Protect(null));
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Protect(string.Empty));
        }

        [TestMethod]
        public void Unprotect_EmptyOrNull_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Unprotect(null));
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Unprotect(string.Empty));
        }

        [TestMethod]
        public void Unprotect_InvalidBase64_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Unprotect("not-valid-base64!!"));
        }

        [TestMethod]
        public void Unprotect_ValidBase64ButNotDpapi_ReturnsEmpty()
        {
            // Valid base64 that was not produced by Protect() must not throw and must return empty.
            var bogus = System.Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(string.Empty, GitHubCopilotAuth.Unprotect(bogus));
        }

        #endregion Protect / Unprotect (DPAPI)

        #region ParseModels

        [TestMethod]
        public void ParseModels_KeepsChatModels_MappingNameAndEndpoint()
        {
            var json = @"{ ""data"": [
                { ""id"": ""gpt-4o"", ""capabilities"": { ""type"": ""chat"" } },
                { ""id"": ""claude-sonnet-4"", ""capabilities"": { ""type"": ""chat"" } }
            ] }";

            var models = GitHubCopilotAuth.ParseModels(json);

            CollectionAssert.AreEqual(
                new[] { "gpt-4o", "claude-sonnet-4" },
                models.Select(m => m.Name).ToArray());
            Assert.IsTrue(models.All(m => m.Endpoint == GitHubCopilotAuth.ApiBaseUrl));
        }

        [TestMethod]
        public void ParseModels_ExcludesNonChatCapabilities()
        {
            var json = @"{ ""data"": [
                { ""id"": ""gpt-4o"", ""capabilities"": { ""type"": ""chat"" } },
                { ""id"": ""text-embedding-3-small"", ""capabilities"": { ""type"": ""embeddings"" } }
            ] }";

            var models = GitHubCopilotAuth.ParseModels(json);

            CollectionAssert.AreEqual(new[] { "gpt-4o" }, models.Select(m => m.Name).ToArray());
        }

        [TestMethod]
        public void ParseModels_ExcludesPickerDisabled()
        {
            var json = @"{ ""data"": [
                { ""id"": ""gpt-4o"", ""model_picker_enabled"": true },
                { ""id"": ""internal-model"", ""model_picker_enabled"": false }
            ] }";

            var models = GitHubCopilotAuth.ParseModels(json);

            CollectionAssert.AreEqual(new[] { "gpt-4o" }, models.Select(m => m.Name).ToArray());
        }

        [TestMethod]
        public void ParseModels_DeduplicatesById_CaseInsensitive()
        {
            var json = @"{ ""data"": [
                { ""id"": ""gpt-4o"" },
                { ""id"": ""GPT-4o"" },
                { ""id"": ""gpt-5"" }
            ] }";

            var models = GitHubCopilotAuth.ParseModels(json);

            CollectionAssert.AreEqual(new[] { "gpt-4o", "gpt-5" }, models.Select(m => m.Name).ToArray());
        }

        [TestMethod]
        public void ParseModels_KeepsModelsWithoutCapabilities()
        {
            // Missing capabilities means we can't prove it's non-chat, so keep it.
            var json = @"{ ""data"": [ { ""id"": ""gpt-5-mini"" } ] }";

            var models = GitHubCopilotAuth.ParseModels(json);

            CollectionAssert.AreEqual(new[] { "gpt-5-mini" }, models.Select(m => m.Name).ToArray());
        }

        [TestMethod]
        public void ParseModels_NullEmptyOrInvalid_ReturnsEmpty()
        {
            Assert.AreEqual(0, GitHubCopilotAuth.ParseModels(null).Count);
            Assert.AreEqual(0, GitHubCopilotAuth.ParseModels(string.Empty).Count);
            Assert.AreEqual(0, GitHubCopilotAuth.ParseModels("not json").Count);
            Assert.AreEqual(0, GitHubCopilotAuth.ParseModels("{ }").Count);
            Assert.AreEqual(0, GitHubCopilotAuth.ParseModels(@"{ ""data"": {} }").Count);
        }

        #endregion ParseModels
    }

    /// <summary>
    /// Tests for the reusable <see cref="SecretProtector"/> — DPAPI roundtrip and the
    /// per-secret entropy separation that lets multiple OAuth providers store tokens safely.
    /// </summary>
    [TestClass]
    public class SecretProtectorTests
    {
        private static readonly byte[] EntropyA = SecretProtector.EntropyFromLabel("provider.A.v1");
        private static readonly byte[] EntropyB = SecretProtector.EntropyFromLabel("provider.B.v1");

        [TestMethod]
        public void ProtectUnprotect_Roundtrip_ReturnsOriginal()
        {
            var secret = "token-abc-123";
            var protectedText = SecretProtector.Protect(secret, EntropyA);

            Assert.AreNotEqual(secret, protectedText);
            Assert.AreEqual(secret, SecretProtector.Unprotect(protectedText, EntropyA));
        }

        [TestMethod]
        public void Unprotect_WithDifferentEntropy_ReturnsEmpty()
        {
            // A secret protected for provider A must NOT be decryptable with provider B's entropy.
            var protectedText = SecretProtector.Protect("token-abc-123", EntropyA);
            Assert.AreEqual(string.Empty, SecretProtector.Unprotect(protectedText, EntropyB));
        }

        [TestMethod]
        public void Protect_EmptyOrNull_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, SecretProtector.Protect(null, EntropyA));
            Assert.AreEqual(string.Empty, SecretProtector.Protect(string.Empty, EntropyA));
        }

        [TestMethod]
        public void Unprotect_EmptyOrInvalid_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, SecretProtector.Unprotect(null, EntropyA));
            Assert.AreEqual(string.Empty, SecretProtector.Unprotect(string.Empty, EntropyA));
            Assert.AreEqual(string.Empty, SecretProtector.Unprotect("not-base64!!", EntropyA));
        }
    }
}

