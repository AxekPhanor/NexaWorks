<Query Kind="Statements">
  <Connection>
    <ID>53732c96-2e36-4c93-99b4-70077a9eac94</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>NexaWorks</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

// Paramètres
var motsClés = Util.ReadLine("Renseigner un mots clés, si vous ne voulez pas renseigner de mot clé laisser le champs vide");
var produit = Util.ReadLine("Renseigner un nom de produit, si vous ne voulez pas renseigner de nom de produit laisser le champs vide");
var date1 = Util.ReadLine("Renseigner la date 1 de la période (aaaa-mm-jj), si vous ne voulez pas renseigner de période laisser le champs vide");
var date2 = "";
if(date1 != "")
{
	date2 = Util.ReadLine("Renseigner la date 2 de la période (aaaa-mm-jj)");
}

DateTime[] periode = null;
if (date1 != "" && date2 != ""){
	periode = new DateTime[2] {
	DateTime.Parse(date1),
	DateTime.Parse(date2)
	};
}

// Requête
var result = Problemes.Where(p => p.Description.Contains(motsClés))
.Join(Statuts, p => p.Statut.Id, s => s.Id, (probleme, statut) => new 
{ 
	Id = probleme.Id, 
	ImplementationId = probleme.ImplementationId, 
	Statut = statut.Nom,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme.Resolution,
	Implementation = probleme.Implementation
})
.Where(p => p.Statut == "Résolu")
.Join(Implementations, p => p.Implementation.Id, i => i.Id, (probleme, implementation) => new 
{
	Id = probleme.Id,
	ProduitVersionId = implementation.ProduitVersionId,
	SystemeExploitationId = implementation.SystemeExploitationId,
	Statut = probleme.Statut,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme.Resolution,
	ProduitVersion = implementation.ProduitVersion,
	SystemeExploitation = implementation.SystemeExploitation
}).Join(SystemeExploitations, p => p.SystemeExploitation.Id, se => se.Id, (probleme, systemeExploitation) => new 
{
	Id = probleme.Id,
	ProduitVersionId = probleme.ProduitVersionId,
	SystemeExploitation = systemeExploitation.Nom,
	Statut = probleme.Statut,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme.Resolution,
	ProduitVersion = probleme.ProduitVersion,
}).Join(ProduitVersions, p => p.ProduitVersion.Id, pv => pv.Id, (probleme, produitVersion) => new
{
	Id = probleme.Id,
	ProduitId = produitVersion.ProduitId,
	VersionId = produitVersion.VersionId,
	SystemeExploitation = probleme.SystemeExploitation,
	Statut = probleme.Statut,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme.Resolution,
	Produit = produitVersion.Produit,
	Version = produitVersion.Version
}).Join(Produits, p => p.Produit.Id, p => p.Id, (probleme, produit) => new
{
	Id = probleme.Id,
	Produit = produit.Nom,
	VersionId = probleme.VersionId,
	SystemeExploitation = probleme.SystemeExploitation,
	Statut = probleme.Statut,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme.Resolution,
	Version = probleme.Version,
}).Where(p => string.IsNullOrEmpty(produit) || p.Produit == produit)
.Join(Versions, p => p.Version.Id, v => v.Id, (probleme, version) => new
{
	Id = probleme.Id,
	Produit = probleme.Produit,
	Version = version.Numero,
	SystemeExploitation = probleme.SystemeExploitation,
	Statut = probleme.Statut,
	DateDeCreation = probleme.DateDeCreation,
	Description = probleme.Description,
	Resolution = probleme
	.Resolution
});

if(periode != null){
	result = result.Where(p => DateTime.Compare(p.DateDeCreation, periode[0]) >= 0 && DateTime.Compare(p.DateDeCreation, periode[1]) <= 0);
}

result.Dump();