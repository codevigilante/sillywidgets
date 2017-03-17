using System;

namespace silly
{
    public static class Globals
    {
        public enum ExitReasons : int { OK = 1, About = 2, UnknownDirective = -1, DirectiveFail = -2  };
        public static string Version = "0.1";
    }
}