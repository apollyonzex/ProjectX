
namespace Foundation {

    public enum Error {
        NoError = 0,
        AssetBundle_LoadFailed = 1,
        Asset_LoadFailed = 2,
        AssetBundle_AsyncLoadNotComplete = 3,
        Asset_AsyncLoadNotComplete = 4,
        AssetBundle_NotLoad = 5,
        Asset_CastTypeFailed = 6,
    }

    public static class ErrorExt {
        public static bool is_ok(this Error self) {
            return self == Error.NoError;
        }
    }
}