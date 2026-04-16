# Instructions de déploiement

Ce projet implémente un serveur web HTTPS minimal en C# (.NET 8) utilisant **Kestrel**, compatible Windows et Linux. Le même code source C# fonctionne sur les deux systèmes sans dépendre de HTTP.sys (Windows) ni d’outils spécifiques.

## Structure du projet
Le fichier devrais contenir les fichiers suivants: `Program.cs`, `HttpsServer.csproj`, `HttpsServer.sln`, `appsettings.json`, `appsettings.Development.json`, `cert.pem`, `cert.pfx` et `key.pem`.

## Prérequis (dépendances)

- **.NET SDK 8.0** (ou plus récent). Installez-le selon votre OS :
  - **Linux (testé sur VM Ubuntu)** : ouvrez un terminal et exécutez :  
    ```bash
    wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt update
    sudo apt install -y dotnet-sdk-8.0
    ```  
    (Ceux-ci utilisent le dépôt Microsoft pour .NET.)  
  - **Windows** : téléchargez et installez le SDK depuis le site officiel [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download/dotnet). Choisissez la version Windows x64 du SDK (.NET 8.0). Démarrez l’installateur téléchargé et suivez les instructions.

- **OpenSSL** (outil en ligne de commande pour générer le certificat) :  
  - **Linux (Ubuntu)** :  
    ```bash
    sudo apt install -y openssl
    ```  
  - **Windows** : installez “Win32/Win64 OpenSSL” (par exemple via [slproweb.com](https://slproweb.com/products/Win32OpenSSL.html)), qui fournit un installeur simple pour OpenSSL sous Windows. Vous pouvez aussi utiliser [chocolatey](https://chocolatey.org/) si disponible, ou tout autre gestionnaire de paquets Windows.


## Génération du certificat HTTPS

Un certificat auto-signé est requis pour le serveur HTTPS. 
Le certificat que j'ai généré est fourni et devrais fonctionner, mais si vous voulez créer votre propre certificat, placez-vous dans le dossier du projet et exécutez les commandes suivantes dans un terminal (Linux) ou PowerShell (Windows) :

```bash
openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365 -nodes -subj "/CN=localhost"
openssl pkcs12 -export -out cert.pfx -inkey key.pem -in cert.pem -passout pass:password
```

- Le premier `openssl req` génère une clé privée `key.pem` et un certificat `cert.pem` valides 1 an avec `CN=localhost`.  
- Le second convertit ces fichiers en un fichier PKCS#12 `cert.pfx` protégé par le mot de passe `password` (mot de passe utilisé dans le code).  

Après cette étape, vous aurez `cert.pfx` dans le dossier du projet.

## Exécution du serveur

Ouvrez un terminal dans le dossier du projet et lancez :  

```bash
dotnet run
```

Vous devriez voir le message **“Now listening on: https://localhost:8443”**. Le serveur est alors actif sur le port 8443 en HTTPS. 

Dans un navigateur, ouvrez <https://localhost:8443>. Le navigateur affichera probablement un avertissement de sécurité (certificat auto-signé), confirmez pour continuer : vous devriez voir la réponse du serveur, c'est à dire une page web avec uniquement le texte "server running https using kestrel" sans mis en forme.


## fichiers exécutables (optionnel)

Pour obtenir un exécutable self contained vous pouvez utiliser :
Sur Linux :
```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```
Sur Windows :
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Cela produit un binaire unique exécutable directement après avoir placé le certificat dans le dossier nouvellement généré (bin/Release/win-x64 ou linux-x64/publish).
