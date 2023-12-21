const baseUrl = 'http://localhost:4200';
export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'Ecommerce Admin',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:5000/',
    redirectUri: baseUrl,
    clientId: 'Ecommerce_Admin',
    dummyClientSecret: '1q2w3e*',
    responseType: 'code',
    scope: 'offline_access Ecommerce.Admin',
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://localhost:5001',
      rootNamespace: 'Ecommerce.Admin',
    },
  },
};
