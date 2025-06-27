// Este arquivo pode ser substituído durante o build por `environment.prod.ts`.
// A lista de arquivos de destino pode ser encontrada em `angular.json`.

export const environment = {
  production: false,
  apiUrl: 'http://localhost:7049/api' // URL da API para desenvolvimento (ajuste a porta se necessário)
};

/*
 * Para facilitar a depuração no modo de desenvolvimento, você pode importar o arquivo abaixo
 * para ignorar frames de pilha de chamadas de erro relacionados à zona, como `zone.run`, `zoneDelegate.invokeTask`.
 *
 * Esta importação deve ser comentada no modo de produção porque terá um impacto negativo
 * no desempenho se um erro for lançado.
 */
// import 'zone.js/plugins/zone-error';  // Incluído por padrão no polyfills.ts do Angular CLI
