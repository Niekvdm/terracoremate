using Newtonsoft.Json.Linq;
using TerracoreMate.Hive.Enums;
using TerracoreMate.Http.Attributes;

namespace TerracoreMate.Http.Services;

public interface IHiveGlobalService
{
    [HivePost]
    [HiveMethod<HiveDatabaseApiMethod>(HiveApiType.DatabaseApi, HiveDatabaseApiMethod.GetDynamicGlobalProperties)]
    Task<JObject> RetrieveDynamicGlobalProperties();
}