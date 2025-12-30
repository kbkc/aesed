# AES Encryption Tool

Einfaches Kommandozeilen-Tool zur AES-256-Verschlüsselung von Passwörtern und sensiblen Strings.

## Funktionsweise

Das Programm arbeitet in zwei Modi:

**Erster Start (keine Dateien vorhanden):**
1. Generiert einen zufälligen AES-256-Schlüssel
2. Speichert den Schlüssel Base64-kodiert in `aes_key`
3. Fragt nach dem zu verschlüsselnden Text
4. Speichert den verschlüsselten Text in `aes_hash`

**Folgende Starts (Dateien vorhanden):**
- Liest Schlüssel und verschlüsselten Text
- Zeigt das entschlüsselte Passwort an

## Verwendung

```bash
dotnet run
```

Beim ersten Start:
```
Enter string to encrypt:
mein_geheimes_passwort
```

Bei vorhandenen Dateien wird das gespeicherte Passwort angezeigt und das Programm beendet sich. Um ein neues Passwort zu verschlüsseln, müssen beide Dateien gelöscht werden.

## Generierte Dateien

| Datei | Inhalt |
|-------|--------|
| `aes_key` | AES-256-Schlüssel (Base64) |
| `aes_hash` | Verschlüsselter Text mit IV (Base64) |

## Technische Details

- **Algorithmus:** AES-256-CBC
- **Padding:** PKCS7
- **IV:** Zufällig generiert, dem Ciphertext vorangestellt
- **Kodierung:** UTF-8 für Klartext, Base64 für Speicherung

## Sicherheitshinweise

- Die Datei `aes_key` enthält den geheimen Schlüssel und muss geschützt werden
- Beide Dateien sollten nicht im selben Verzeichnis wie öffentlich zugänglicher Code liegen
- Für Produktionsumgebungen sollte der Schlüssel in einem sicheren Speicher (z.B. Windows DPAPI, Azure Key Vault) abgelegt werden

## Voraussetzungen

- .NET 9.0 oder höher
- Terminal mit ANSI-Farbunterstützung (optional, für farbige Ausgabe)

## Lizenz

b1b17@outlook.com, 2025
