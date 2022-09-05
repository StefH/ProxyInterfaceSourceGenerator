//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace ProxyInterfaceSourceGeneratorTests.Source.PnP
{
    public partial interface IWeb
    {
        new Microsoft.SharePoint.Client.Web _Instance { get; }

        string AccessRequestListUrl { get; }

        string AccessRequestSiteDescription { get; }

        string Acronym { get; }

        Microsoft.SharePoint.Client.AlertCollection Alerts { get; }

        bool AllowAutomaticASPXPageIndexing { get; set; }

        bool AllowCreateDeclarativeWorkflowForCurrentUser { get; }

        bool AllowDesignerForCurrentUser { get; }

        bool AllowMasterPageEditingForCurrentUser { get; }

        bool AllowRevertFromTemplateForCurrentUser { get; }

        bool AllowRssFeeds { get; }

        bool AllowSaveDeclarativeWorkflowAsTemplateForCurrentUser { get; }

        bool AllowSavePublishDeclarativeWorkflowForCurrentUser { get; }

        Microsoft.SharePoint.Client.PropertyValues AllProperties { get; }

        string AlternateCssUrl { get; set; }

        System.Guid AppInstanceId { get; }

        Microsoft.SharePoint.Client.AppTileCollection AppTiles { get; }

        Microsoft.SharePoint.Client.Group AssociatedMemberGroup { get; set; }

        Microsoft.SharePoint.Client.Group AssociatedOwnerGroup { get; set; }

        Microsoft.SharePoint.Client.Group AssociatedVisitorGroup { get; set; }

        Microsoft.SharePoint.Client.User Author { get; }

        Microsoft.SharePoint.Client.ContentTypeCollection AvailableContentTypes { get; }

        Microsoft.SharePoint.Client.FieldCollection AvailableFields { get; }

        Microsoft.SharePoint.Client.ModernizeHomepageResult CanModernizeHomepage { get; }

        string ClassicWelcomePage { get; set; }

        bool CommentsOnSitePagesDisabled { get; set; }

        short Configuration { get; }

        bool ContainsConfidentialInfo { get; set; }

        Microsoft.SharePoint.Client.ContentTypeCollection ContentTypes { get; }

        System.DateTime Created { get; }

        Microsoft.SharePoint.Client.ChangeToken CurrentChangeToken { get; }

        Microsoft.SharePoint.Client.User CurrentUser { get; }

        string CustomMasterUrl { get; set; }

        bool CustomSiteActionsDisabled { get; set; }

        Microsoft.SharePoint.Client.SPDataLeakagePreventionStatusInfo DataLeakagePreventionStatusInfo { get; }

        string Description { get; set; }

        string DescriptionForExistingLanguage { get; set; }

        Microsoft.SharePoint.Client.UserResource DescriptionResource { get; }

        System.Collections.Generic.IEnumerable<Microsoft.SharePoint.Client.SPResourceEntry> DescriptionTranslations { get; set; }

        string DesignerDownloadUrlForCurrentUser { get; }

        System.Guid DesignPackageId { get; set; }

        bool DisableAppViews { get; set; }

        bool DisableFlows { get; set; }

        bool DisableRecommendedItems { get; set; }

        bool DocumentLibraryCalloutOfficeWebAppPreviewersDisabled { get; }

        Microsoft.SharePoint.Client.BasePermissions EffectiveBasePermissions { get; }

        bool EnableMinimalDownload { get; set; }

        Microsoft.SharePoint.Client.EventReceiverDefinitionCollection EventReceivers { get; }

        bool ExcludeFromOfflineClient { get; set; }

        Microsoft.SharePoint.Client.FeatureCollection Features { get; }

        Microsoft.SharePoint.Client.FieldCollection Fields { get; }

        Microsoft.SharePoint.Client.FolderCollection Folders { get; }

        Microsoft.SharePoint.Client.FooterVariantThemeType FooterEmphasis { get; set; }

        bool FooterEnabled { get; set; }

        Microsoft.SharePoint.Client.FooterLayoutType FooterLayout { get; set; }

        bool HasWebTemplateExtension { get; set; }

        Microsoft.SharePoint.Client.SPVariantThemeType HeaderEmphasis { get; set; }

        Microsoft.SharePoint.Client.HeaderLayoutType HeaderLayout { get; set; }

        bool HideTitleInHeader { get; set; }

        bool HorizontalQuickLaunch { get; set; }

        Microsoft.SharePoint.ClientSideComponent.HostedAppsManager HostedApps { get; }

        System.Guid Id { get; }

        bool IsEduClass { get; }

        bool IsEduClassProvisionChecked { get; }

        bool IsEduClassProvisionPending { get; }

        bool IsHomepageModernized { get; }

        bool IsMultilingual { get; set; }

        bool IsProvisioningComplete { get; }

        bool IsRevertHomepageLinkHidden { get; set; }

        uint Language { get; }

        System.DateTime LastItemModifiedDate { get; }

        System.DateTime LastItemUserModifiedDate { get; }

        Microsoft.SharePoint.Client.ListCollection Lists { get; }

        Microsoft.SharePoint.Client.ListTemplateCollection ListTemplates { get; }

        Microsoft.SharePoint.Client.LogoAlignment LogoAlignment { get; set; }

        string MasterUrl { get; set; }

        bool MegaMenuEnabled { get; set; }

        bool MembersCanShare { get; set; }

        bool NavAudienceTargetingEnabled { get; set; }

        Microsoft.SharePoint.Client.Navigation Navigation { get; }

        bool NextStepsFirstRunEnabled { get; set; }

        bool NoCrawl { get; set; }

        bool NotificationsInOneDriveForBusinessEnabled { get; }

        bool NotificationsInSharePointEnabled { get; }

        bool ObjectCacheEnabled { get; set; }

        bool OverwriteTranslationsOnChange { get; set; }

        Microsoft.SharePoint.Client.WebInformation ParentWeb { get; }

        Microsoft.SharePoint.Client.ResourcePath ResourcePath { get; }

        bool PreviewFeaturesEnabled { get; }

        string PrimaryColor { get; }

        Microsoft.SharePoint.Client.PushNotificationSubscriberCollection PushNotificationSubscribers { get; }

        bool QuickLaunchEnabled { get; set; }

        Microsoft.SharePoint.Client.RecycleBinItemCollection RecycleBin { get; }

        bool RecycleBinEnabled { get; }

        Microsoft.SharePoint.Client.RegionalSettings RegionalSettings { get; }

        string RequestAccessEmail { get; set; }

        Microsoft.SharePoint.Client.RoleDefinitionCollection RoleDefinitions { get; }

        Microsoft.SharePoint.Client.Folder RootFolder { get; }

        bool SaveSiteAsTemplateEnabled { get; set; }

        Microsoft.SharePoint.Client.SearchBoxInNavBarType SearchBoxInNavBar { get; set; }

        string SearchBoxPlaceholderText { get; set; }

        Microsoft.SharePoint.Client.SearchScopeType SearchScope { get; set; }

        Microsoft.SharePoint.Client.ResourcePath ServerRelativePath { get; }

        string ServerRelativeUrl { get; set; }

        bool ShowUrlStructureForCurrentUser { get; }

        Microsoft.SharePoint.Marketplace.CorporateCuratedGallery.SiteCollectionCorporateCatalogAccessor SiteCollectionAppCatalog { get; }

        Microsoft.SharePoint.Client.GroupCollection SiteGroups { get; }

        string SiteLogoDescription { get; set; }

        string SiteLogoUrl { get; set; }

        Microsoft.SharePoint.Client.List SiteUserInfoList { get; }

        Microsoft.SharePoint.Client.UserCollection SiteUsers { get; }

        System.Collections.Generic.IEnumerable<int> SupportedUILanguageIds { get; set; }

        bool SyndicationEnabled { get; set; }

        Microsoft.SharePoint.Client.SharingState TenantAdminMembersCanShare { get; }

        Microsoft.SharePoint.Marketplace.CorporateCuratedGallery.TenantCorporateCatalogAccessor TenantAppCatalog { get; }

        bool TenantTagPolicyEnabled { get; }

        string ThemedCssFolderUrl { get; set; }

        Microsoft.SharePoint.Client.ThemeInfo ThemeInfo { get; }

        bool ThirdPartyMdmEnabled { get; }

        string Title { get; set; }

        string TitleForExistingLanguage { get; set; }

        Microsoft.SharePoint.Client.UserResource TitleResource { get; }

        System.Collections.Generic.IEnumerable<Microsoft.SharePoint.Client.SPResourceEntry> TitleTranslations { get; set; }

        bool TreeViewEnabled { get; set; }

        int UIVersion { get; set; }

        bool UIVersionConfigurationEnabled { get; set; }

        string Url { get; }

        bool UseAccessRequestDefault { get; }

        Microsoft.SharePoint.Client.UserCustomActionCollection UserCustomActions { get; }

        Microsoft.SharePoint.Client.WebCollection Webs { get; }

        string WebTemplate { get; }

        string WebTemplateConfiguration { get; }

        bool WebTemplatesGalleryFirstRunEnabled { get; set; }

        string WelcomePage { get; }

        Microsoft.SharePoint.Client.Workflow.WorkflowAssociationCollection WorkflowAssociations { get; }

        Microsoft.SharePoint.Client.Workflow.WorkflowTemplateCollection WorkflowTemplates { get; }



        System.Uri WebUrlFromPageUrlDirect(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext context, System.Uri pageFullUrl);

        System.Uri WebUrlFromFolderUrlDirect(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientContext context, System.Uri folderFullUrl);

        Microsoft.SharePoint.Client.ClientResult<bool> DoesUserHavePermissions(Microsoft.SharePoint.Client.BasePermissions permissionMask);

        Microsoft.SharePoint.Client.ClientResult<Microsoft.SharePoint.Client.BasePermissions> GetUserEffectivePermissions(string userName);

        void CreateDefaultAssociatedGroups(string userLogin, string userLogin2, string groupNameSeed);

        Microsoft.SharePoint.Client.ClientResult<string> CreateOrganizationSharingLink(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, bool isEditLink);

        void DestroyOrganizationSharingLink(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, bool isEditLink, bool removeAssociatedSharingLinkGroup);

        Microsoft.SharePoint.Client.ClientResult<Microsoft.SharePoint.Client.SharingLinkKind> GetSharingLinkKind(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string fileUrl);

        Microsoft.SharePoint.Client.ClientResult<Microsoft.SharePoint.Client.SharingLinkData> GetSharingLinkData(string linkUrl);

        Microsoft.SharePoint.Client.ClientResult<string> MapToIcon(string fileName, string progId, Microsoft.SharePoint.Client.Utilities.IconSize size);

        Microsoft.SharePoint.Client.ClientResult<string> GetWebUrlFromPageUrl(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string pageFullUrl);

        Microsoft.SharePoint.Client.PushNotificationSubscriber RegisterPushNotificationSubscriber(System.Guid deviceAppInstanceId, string serviceToken);

        void UnregisterPushNotificationSubscriber(System.Guid deviceAppInstanceId);

        Microsoft.SharePoint.Client.PushNotificationSubscriberCollection GetPushNotificationSubscribersByArgs(string customArgs);

        Microsoft.SharePoint.Client.PushNotificationSubscriberCollection GetPushNotificationSubscribersByUser(string userName);

        Microsoft.SharePoint.Client.ClientResult<bool> DoesPushNotificationSubscriberExist(System.Guid deviceAppInstanceId);

        Microsoft.SharePoint.Client.PushNotificationSubscriber GetPushNotificationSubscriber(System.Guid deviceAppInstanceId);

        Microsoft.SharePoint.Client.User GetSiteUserIncludingDeletedByPuid(string puid);

        Microsoft.SharePoint.Client.User GetUserById(int userId);

        Microsoft.SharePoint.Client.ClientResult<bool> EnsureTenantAppCatalog(string callerId);

        Microsoft.SharePoint.ClientSideComponent.StorageEntity GetStorageEntity(string key);

        void SetStorageEntity(string key, string value, string description, string comments);

        void RemoveStorageEntity(string key);

        Microsoft.SharePoint.Client.SharingResult ShareObject(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, string peoplePickerInput, string roleValue, int groupId, bool propagateAcl, bool sendEmail, bool includeAnonymousLinkInEmail, string emailSubject, string emailBody, bool useSimplifiedRoles);

        Microsoft.SharePoint.Client.SharingResult ForwardObjectLink(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, string peoplePickerInput, string emailSubject, string emailBody);

        Microsoft.SharePoint.Client.SharingResult UnshareObject(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url);

        Microsoft.SharePoint.Client.ObjectSharingSettings GetObjectSharingSettings(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string objectUrl, int groupId, bool useSimplifiedRoles);

        Microsoft.SharePoint.Client.ClientResult<string> CreateAnonymousLink(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, bool isEditLink);

        Microsoft.SharePoint.Client.ClientResult<string> CreateAnonymousLinkWithExpiration(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, bool isEditLink, string expirationString);

        void DeleteAllAnonymousLinksForObject(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url);

        void DeleteAnonymousLinkForObject(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string url, bool isEditLink, bool removeAssociatedSharingLinkGroup);

        Microsoft.SharePoint.Client.ListCollection GetLists(Microsoft.SharePoint.Client.GetListsParameters getListsParams);

        Microsoft.SharePoint.Client.WebTemplateCollection GetAvailableWebTemplates(uint lcid, bool doIncludeCrossLanguage);

        Microsoft.SharePoint.Client.List GetCatalog(int typeCatalog);

        Microsoft.SharePoint.Client.RecycleBinItemCollection GetRecycleBinItems(string pagingInfo, int rowLimit, bool isAscending, Microsoft.SharePoint.Client.RecycleBinOrderBy orderBy, Microsoft.SharePoint.Client.RecycleBinItemState itemState);

        Microsoft.SharePoint.Client.RecycleBinItemCollection GetRecycleBinItemsByQueryInfo(Microsoft.SharePoint.Client.RecycleBinQueryInformation queryInfo);

        Microsoft.SharePoint.Client.ChangeCollection GetChanges(Microsoft.SharePoint.Client.ChangeQuery query);

        Microsoft.SharePoint.Client.List GetList(string strUrl);

        Microsoft.SharePoint.Client.List GetListUsingPath(Microsoft.SharePoint.Client.ResourcePath path);

        Microsoft.SharePoint.Client.ListItem GetListItem(string strUrl);

        Microsoft.SharePoint.Client.ListItem GetListItemUsingPath(Microsoft.SharePoint.Client.ResourcePath path);

        Microsoft.SharePoint.Client.ListItem GetListItemByResourceId(string resourceId);

        Microsoft.BusinessData.MetadataModel.Entity GetEntity(string @namespace, string name);

        Microsoft.BusinessData.MetadataModel.AppBdcCatalog GetAppBdcCatalogForAppInstance(System.Guid appInstanceId);

        Microsoft.BusinessData.MetadataModel.AppBdcCatalog GetAppBdcCatalog();

        Microsoft.SharePoint.Client.WebCollection GetSubwebsForCurrentUser(Microsoft.SharePoint.Client.SubwebQuery query);

        Microsoft.SharePoint.Client.ClientResult<System.IO.Stream> GetSPAppContextAsStream();

        void Update();

        Microsoft.SharePoint.Client.View GetViewFromUrl(string listUrl);

        Microsoft.SharePoint.Client.View GetViewFromPath(Microsoft.SharePoint.Client.ResourcePath listPath);

        Microsoft.SharePoint.Client.File GetFileByServerRelativeUrl(string serverRelativeUrl);

        Microsoft.SharePoint.Client.File GetFileByServerRelativePath(Microsoft.SharePoint.Client.ResourcePath serverRelativePath);

        System.Collections.Generic.IList<Microsoft.SharePoint.Client.DocumentLibraryInformation> GetDocumentLibraries(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string webFullUrl);

        System.Collections.Generic.IList<Microsoft.SharePoint.Client.DocumentLibraryInformation> GetDocumentAndMediaLibraries(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string webFullUrl, bool includePageLibraries);

        Microsoft.SharePoint.Client.ClientResult<Microsoft.SharePoint.Client.DocumentLibraryInformation> DefaultDocumentLibraryUrl(ProxyInterfaceSourceGeneratorTests.Source.PnP.IClientRuntimeContext context, string webUrl);

        Microsoft.SharePoint.Client.List DefaultDocumentLibrary();

        Microsoft.SharePoint.Client.File GetFileById(System.Guid uniqueId);

        Microsoft.SharePoint.Client.Folder GetFolderById(System.Guid uniqueId);

        Microsoft.SharePoint.Client.File GetFileByLinkingUrl(string linkingUrl);

        Microsoft.SharePoint.Client.File GetFileByGuestUrl(string guestUrl);

        Microsoft.SharePoint.Client.File GetFileByGuestUrlEnsureAccess(string guestUrl, bool ensureAccess);

        Microsoft.SharePoint.Client.File GetFileByWOPIFrameUrl(string wopiFrameUrl);

        Microsoft.SharePoint.Client.File GetFileByUrl(string fileUrl);

        Microsoft.SharePoint.Client.Folder GetFolderByServerRelativeUrl(string serverRelativeUrl);

        Microsoft.SharePoint.Client.Folder GetFolderByServerRelativePath(Microsoft.SharePoint.Client.ResourcePath serverRelativePath);

        void ApplyWebTemplate(string webTemplate);

        void DeleteObject();

        Microsoft.SharePoint.Client.ClientResult<System.IO.Stream> PageContextInfo(bool includeODBSettings, bool emitNavigationInfo);

        Microsoft.SharePoint.Client.ClientResult<System.IO.Stream> PageContextCore();

        Microsoft.SharePoint.Client.AppInstance GetAppInstanceById(System.Guid appInstanceId);

        Microsoft.SharePoint.Client.ClientObjectList<Microsoft.SharePoint.Client.AppInstance> GetAppInstancesByProductId(System.Guid productId);

        Microsoft.SharePoint.Client.AppInstance LoadAndInstallAppInSpecifiedLocale(System.IO.Stream appPackageStream, int installationLocaleLCID);

        Microsoft.SharePoint.Client.AppInstance LoadApp(System.IO.Stream appPackageStream, int installationLocaleLCID);

        Microsoft.SharePoint.Client.ClientResult<System.Guid> AddPlaceholderUser(string listId, string placeholderText);

        Microsoft.SharePoint.Client.AppInstance LoadAndInstallApp(System.IO.Stream appPackageStream);

        void SetAccessRequestSiteDescriptionAndUpdate(string description);

        void SetUseAccessRequestDefaultAndUpdate(bool useAccessRequestDefault);

        void IncrementSiteClientTag();

        void AddSupportedUILanguage(int lcid);

        void RemoveSupportedUILanguage(int lcid);

        Microsoft.SharePoint.Client.User EnsureUser(string logonName);

        Microsoft.SharePoint.Client.User EnsureUserByObjectId(System.Guid objectId, System.Guid tenantId, Microsoft.SharePoint.Client.Utilities.PrincipalType principalType);

        void ApplyTheme(string colorPaletteUrl, string fontSchemeUrl, string backgroundImageUrl, bool shareGenerated);




    }
}
#nullable disable