using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmploiASP_2;

namespace EmploiASP_2.Controllers
{
    public class ContratsController : Controller
    {
        private DBIG3B7Entities db = new DBIG3B7Entities();

        // GET: Contrats
        public ActionResult Index()
        {
            var contrat = db.Contrat.Include(c => c.EntrepriseClient).Include(c => c.ContratTravailleurNonSoumis).Include(c => c.ContratTravailleurSoumis).Include(c => c.Profession).Include(c => c.Travailleur);
            return View(contrat.ToList());
        }

        // GET: Contrats/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contrat contrat = db.Contrat.Find(id);
            if (contrat == null)
            {
                return HttpNotFound();
            }
            return View(contrat);
        }

        // GET: Contrats/Create
        public ActionResult Create()
        {
            ViewBag.idContrat = new SelectList(db.ContratTravailleurNonSoumis, "idContrat", "idContrat");
            ViewBag.idContrat = new SelectList(db.ContratTravailleurSoumis, "idContrat", "numDossierMedical");
            // Pour la récupération dans un dropDownList, la valeur sera idTravailleur et le libelle sera le nom
            ViewBag.idTravailleur = new SelectList(db.Travailleur, "idTravailleur", "nom");
            
            List<SelectListItem> profession = (from p in db.Profession.ToList()
                                               select new SelectListItem()
                                               {
                                                   Value = p.codeAlphabetique,
                                                   Text = p.TraductionProfession.Single(t => t.Langue.libelle == "fr").TexteTradProfession
                                               }).ToList();
            ViewBag.codeAlphabetique = profession;

            List<SelectListItem> entrepriseClient = (from ec in db.EntrepriseClient.ToList()
                                               select new SelectListItem()
                                               {
                                                   Value = ec.numero.ToString(),
                                                   Text = ec.TraductionEntreprise.Single(t => t.Langue.libelle == "fr").tradTexteEntreprise
                                               }).ToList();
            ViewBag.numero = entrepriseClient;

            return View();
        }

        // POST: Contrats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "contratTravailleurSoumis,idContrat,dateEntree,idTravailleur,numero,dateSortie,codeAlphabetique")] Contrat contrat)
        {
            // methode appelée lorsqu'on retourne sur la page alors qu'il y a eu une erreur ou via un retour en arriere
            // binding automatique des valeurs
            if (contrat.dateSortie != null && contrat.dateEntree.CompareTo(contrat.dateSortie) > 0)
            {
                ViewBag.error = "Date d'entrée plus grande que date de sortie";
            }
            else if (ModelState.IsValid)
            {
                // on enregistre la valeur du contrat soumis passé dans le formulaire
                ContratTravailleurSoumis saveContratTravSoumis = contrat.ContratTravailleurSoumis;
                // on efface car foreign key non existante
                contrat.ContratTravailleurSoumis = null;
                db.Contrat.Add(contrat);
                db.SaveChanges();
                // on test pour savoir quel type de contrat a été choisi par l'user puis on l'ajoute
                if (saveContratTravSoumis.idContrat == 1111)
                {
                    ContratTravailleurSoumis cts = new ContratTravailleurSoumis();
                    cts.idContrat = db.Contrat.Max(c => c.idContrat);
                    cts.numDossierMedical = saveContratTravSoumis.numDossierMedical;
                    db.ContratTravailleurSoumis.Add(cts);
                }
                else
                {
                    ContratTravailleurNonSoumis ctns = new ContratTravailleurNonSoumis();
                    ctns.idContrat = db.Contrat.Max(c => c.idContrat);
                    db.ContratTravailleurNonSoumis.Add(ctns);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.idContrat = new SelectList(db.ContratTravailleurNonSoumis, "idContrat", "idContrat", contrat.idContrat);
            ViewBag.idContrat = new SelectList(db.ContratTravailleurSoumis, "idContrat", "numDossierMedical", contrat.idContrat);
            ViewBag.idTravailleur = new SelectList(db.Travailleur, "idTravailleur", "nom", contrat.idTravailleur);

            List<SelectListItem> profession = (from p in db.Profession.ToList()
                                               select new SelectListItem()
                                               {
                                                   Value = p.codeAlphabetique,
                                                   Text = p.TraductionProfession.Single(t => t.Langue.libelle == "fr").TexteTradProfession
                                               }).ToList();
            ViewBag.codeAlphabetique = profession;

            List<SelectListItem> entrepriseClient = (from ec in db.EntrepriseClient.ToList()
                                                     select new SelectListItem()
                                                     {
                                                         Value = ec.numero.ToString(),
                                                         Text = ec.TraductionEntreprise.Single(t => t.Langue.libelle == "fr").tradTexteEntreprise
                                                     }).ToList();
            ViewBag.numero = entrepriseClient;
            return View(contrat);
        }

        // GET: Contrats/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contrat contrat = db.Contrat.Find(id);
            if (contrat == null)
            {
                return HttpNotFound();
            }
            ViewBag.numero = new SelectList(db.EntrepriseClient, "numero", "denomination", contrat.numero);
            ViewBag.idContrat = new SelectList(db.ContratTravailleurNonSoumis, "idContrat", "idContrat", contrat.idContrat);
            ViewBag.idContrat = new SelectList(db.ContratTravailleurSoumis, "idContrat", "numDossierMedical", contrat.idContrat);
            ViewBag.codeAlphabetique = new SelectList(db.Profession, "codeAlphabetique", "codeAlphabetique", contrat.codeAlphabetique);
            ViewBag.idTravailleur = new SelectList(db.Travailleur, "idTravailleur", "nom", contrat.idTravailleur);
            return View(contrat);
        }

        // POST: Contrats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idContrat,dateEntree,idTravailleur,numero,dateSortie,codeAlphabetique")] Contrat contrat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contrat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.numero = new SelectList(db.EntrepriseClient, "numero", "denomination", contrat.numero);
            ViewBag.idContrat = new SelectList(db.ContratTravailleurNonSoumis, "idContrat", "idContrat", contrat.idContrat);
            ViewBag.idContrat = new SelectList(db.ContratTravailleurSoumis, "idContrat", "numDossierMedical", contrat.idContrat);
            ViewBag.codeAlphabetique = new SelectList(db.Profession, "codeAlphabetique", "codeAlphabetique", contrat.codeAlphabetique);
            ViewBag.idTravailleur = new SelectList(db.Travailleur, "idTravailleur", "nom", contrat.idTravailleur);
            return View(contrat);
        }

        // GET: Contrats/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contrat contrat = db.Contrat.Find(id);
            if (contrat == null)
            {
                return HttpNotFound();
            }
            return View(contrat);
        }

        // POST: Contrats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Contrat contrat = db.Contrat.Find(id);
            db.Contrat.Remove(contrat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
