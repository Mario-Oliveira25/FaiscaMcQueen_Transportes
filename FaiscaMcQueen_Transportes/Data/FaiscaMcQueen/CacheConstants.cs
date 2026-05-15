        namespace FaiscaMcQueen_Transportes.Infrastructure
    {
        
        public static class CacheConstants
        {
            // Cache Durations (em minutos)
            public const int CACHE_DURATION_SHORT = 15;      // 15 minutos para dados que mudam frequentemente
            public const int CACHE_DURATION_MEDIUM = 30;     // 30 minutos para dados normais
            public const int CACHE_DURATION_LONG = 60;       // 60 minutos para dados que mudam raramente

            // Response Cache Durations (em segundos)
            public const int RESPONSE_CACHE_SHORT = 300;     // 5 minutos
            public const int RESPONSE_CACHE_MEDIUM = 600;    // 10 minutos
            public const int RESPONSE_CACHE_LONG = 1800;     // 30 minutos

            // Ativos Cache Keys
            public const string CACHE_ATIVOS_LIST = "ativos_list";
            public const string CACHE_ATIVO_DETAILS = "ativo_details_{0}";

            // Técnicos Cache Keys
            public const string CACHE_TECNICOS_LIST = "tecnicos_list";
            public const string CACHE_TECNICO_DETAILS = "tecnico_details_{0}";

            // Intervenções Cache Keys
            public const string CACHE_INTERVENCOES_LIST = "intervencoes_list";
            public const string CACHE_INTERVENCAO_DETAILS = "intervencao_details_{0}";
            public const string CACHE_ATIVOS_FOR_INTERVENCAO = "ativos_for_intervencao";
            public const string CACHE_TECNICOS_FOR_INTERVENCAO = "tecnicos_for_intervencao";
        }
    }

