export interface IUserPermissions {
    canAccessClientAdmin: boolean;
    canEditClients: boolean;
    canEditGroups: boolean;
    canDeleteArchives: boolean;
    clientConfiguratorClaims: string[];
}
