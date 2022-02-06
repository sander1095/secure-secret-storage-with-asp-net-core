$ErrorActionPreference = "Stop"

Write-Host "Azure Resources will now be created."

$account = az account show | ConvertFrom-Json
$subscriptionId = $account.id
$signedInUser = az ad signed-in-user show | ConvertFrom-Json
$signedInUserObjectId = $signedInUser.objectId
$signedInUserPrincipalName = $signedInUser.userPrincipalName

$resourceGroupName = "rg-secret-storage-presentation"
$managedIdentityName = "mi-secret-storage-presentation"
$keyVaultName = "kv-secret-storage"
$databaseName = "db-pizza"
$sqlServerName = "sql-secret-storage-presentation"

Write-Host "Creating resource group"

az group create --location westeurope --name $resourceGroupName

Write-Host "Creating Identity"

# Create the managed identity
$identity = az identity create `
    --name $managedIdentityName `
    --resource-group $resourceGroupName | ConvertFrom-Json

Write-Host "Create keyvault"

# Create a keyvault for the backend to store its secrets
az keyvault create `
    --name $keyVaultName `
    --resource-group $resourceGroupName `
    --location westeurope `
    --enabled-for-deployment true `
    --enabled-for-template-deployment true `
    --enable-soft-delete true `
    --sku standard

Write-Host "Assigning KeyVault permissions for Managed Identity"

az keyvault set-policy `
    --name $keyVaultName `
    --certificate-permissions get list `
    --key-permissions get list `
    --secret-permissions get list `
    --resource-group $resourceGroupName `
    --object-id $identity.principalId 

Write-Host "Setting secrets in keyvault"

az keyvault secret set `
    --name "ConnectionStrings--Database4" `
    --value "Server=tcp:sql-secret-storage-presentation.database.windows.net,1433;Initial Catalog=db-pizza;Persist Security Info=False;Authentication=Active Directory Default;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" `
    --vault-name $keyVaultName

az keyvault secret set `
    --vault-name $keyVaultName `
    --name "SomeExternalApi--ApiKey" `
    --value "h*5*ggh4*FGfg4"

Write-Host "Create SQL Server"

az sql server create `
    --name $sqlServerName `
    --resource-group $resourceGroupName `
    --enable-ad-only-auth `
    --external-admin-principal-type User `
    --external-admin-name $signedInUserPrincipalName `
    --external-admin-sid $signedInUserObjectId `
    # Can't get this to work..
    #--user-assigned-identity-id $identity.id `
    #--identity-type UserAssigned `
    #--pid $identity.id

Write-Host "You still need to assign the Managed Identity in SQL Server yourself! Also configure RBAC!"

Write-Host "Create database"

az sql db create `
    --name $databaseName `
    --resource-group $resourceGroupName `
    --server $sqlServerName `
    --collation SQL_Latin1_General_CP1_CI_AS `
    --edition Basic `
    --service-objective Basic
    --zone-redundant false

Write-Host "For full managed identity auth you might still need to add the managed identity user to the database. This is outside of this demo for now"
