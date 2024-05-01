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
var motsClés = Util.ReadLine("Mots clés (optionnel): ");
var produit = Util.ReadLine("Nom produit (optionnel): ");

var date1 = Util.ReadLine("Période date 1 - optionnel (aaaa-mm-jj): ");
var date2 = Util.ReadLine("Période date 2 - optionnel (aaaa-mm-jj): ");

DateTime[] periode = null;
if (date1 != "" || date2 != ""){
	periode = new DateTime[2] {
	DateTime.Parse(date1),
	DateTime.Parse(date2)
	};
}

// Requête
var result = Tickets.Where(t => t.Probleme.Contains(motsClés))
.Join(Statuts, t => t.Statut.Id, s => s.Id, (ticket, statut) => new 
{ 
	Id = ticket.Id, 
	ImplementationId = ticket.ImplementationId, 
	Statut = statut.Etat,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution,
	Implementation = ticket.Implementation
})
.Where(t => t.Statut == false)
.Join(Implementations, t => t.Implementation.Id, i => i.Id, (ticket, implementation) => new 
{
	Id = ticket.Id,
	ProduitVersionId = implementation.ProduitVersionId,
	SystemeExploitationId = implementation.SystemeExploitationId,
	Statut = ticket.Statut,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution,
	ProduitVersion = implementation.ProduitVersion,
	SystemeExploitation = implementation.SystemeExploitation
}).Join(SystemeExploitations, t => t.SystemeExploitation.Id, se => se.Id, (ticket, systemeExploitation) => new 
{
	Id = ticket.Id,
	ProduitVersionId = ticket.ProduitVersionId,
	SystemeExploitation = systemeExploitation.Nom,
	Statut = ticket.Statut,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution,
	ProduitVersion = ticket.ProduitVersion,
}).Join(ProduitVersions, t => t.ProduitVersion.Id, pv => pv.Id, (ticket, produitVersion) => new
{
	Id = ticket.Id,
	ProduitId = produitVersion.ProduitId,
	VersionId = produitVersion.VersionId,
	SystemeExploitation = ticket.SystemeExploitation,
	Statut = ticket.Statut,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution,
	Produit = produitVersion.Produit,
	Version = produitVersion.Version
}).Join(Produits, t => t.Produit.Id, p => p.Id, (ticket, produit) => new
{
	Id = ticket.Id,
	Produit = produit.Nom,
	VersionId = ticket.VersionId,
	SystemeExploitation = ticket.SystemeExploitation,
	Statut = ticket.Statut,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution,
	Version = ticket.Version,
}).Where(t => string.IsNullOrEmpty(produit) || t.Produit == produit)
.Join(Versions, t => t.Version.Id, v => v.Id, (ticket, version) => new
{
	Id = ticket.Id,
	Produit = ticket.Produit,
	Version = version.Numero,
	SystemeExploitation = ticket.SystemeExploitation,
	Statut = ticket.Statut,
	DateDeCreation = ticket.DateDeCreation,
	Probleme = ticket.Probleme,
	Resolution = ticket.Resolution
});

if(periode != null){
	result = result.Where(t => DateTime.Compare(t.DateDeCreation, periode[0]) >= 0 && DateTime.Compare(t.DateDeCreation, periode[1]) <= 0);
}

result.Dump();