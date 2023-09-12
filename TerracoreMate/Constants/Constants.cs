namespace TerracoreMate;

public static class Constants
{
    public static class Application
    {
        public const int ActivityTimeInterval = 1000 * 30; // 30s
        public const int DefaultTaskDelay = 5000; // 5s

        public const int BattleTransactionValidationAttempts = 5;
        public const int BossTransactionValidationAttempts = 5;
        public const int ClaimTransactionValidationAttempts = 5;
        public const int UpgradeTransactionValidationAttempts = 5;
    }

    public static class TransactionIds
    {
        public const string Claim = "terracore_claim";
        public const string Battle = "terracore_battle";
        public const string Equip = "terracore_equip";
        public const string Unequip = "terracore_unequip";
        public const string BuyCrate = "terracore_buy_crate";
        public const string OpenCrate = "terracore_open_crate";

        public const string HiveMainTransaction = "ssc-mainnet-hive";

        public static class Prefixes
        {
            public const string Terracore = "terracore";
            public const string BossFight = $"{Terracore}_boss_fight";
            public const string BuyCrate = "tm_buy_crate";
            public const string Stake = "stake";
        }
    }

    public static class Terracore
    {
        public const int RegistrationProtectionTime = 86400000; // 24h
        public const int LastBattleProtectionTime = 60000; // 1m

        // Example seed: "04ab389f26f45d26cf56b06c6eb4bf8bd69cfeb6@9c5bb620a425bc81a26f8b558e079e9bae58b918@1694487145129"
        // Seed will be split on '@' and the index below will be used to acquire the correct part of the string
        public const int SeedTransactionIdIndex = 1;
    }
}