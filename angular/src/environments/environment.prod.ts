import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Admin',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44375/',
    redirectUri: baseUrl,
    clientId: 'Admin_App',
    responseType: 'code',
    scope: 'offline_access Admin',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44365',
      rootNamespace: 'Ecommerce.Admin',
    },
  },
} as Environment;
