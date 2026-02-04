# S7Dashboard

Echtzeit-Visualisierung von Siemens S7-1500 SPS-Daten als konfigurierbare Kacheln im Browser.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4)

## Features

- **Echtzeit-Datenvisualisierung** - Polling von S7-1500 Datenbausteinen
- **Konfigurierbare Kacheln** - Größe, Schriftart, Farben individuell einstellbar
- **Mehrere Dashboards** - Verschiedene Ansichten mit eigenen Kacheln
- **Grenzwert-Farben** - Hintergrundfarbe ändert sich basierend auf Wertgrenzen
- **PIN-Schutz** - Einstellungen optional mit PIN schützen
- **Einklappbare Seitenleiste** - Mehr Platz für das Dashboard
- **Netzwerkzugriff** - Von jedem Gerät im Netzwerk erreichbar

## Installation

### Portable Version
1. Release herunterladen
2. In beliebigen Ordner entpacken
3. `S7Dashboard starten.bat` ausführen
4. Browser öffnen: http://localhost:5000

### Aus Quellcode
```bash
git clone https://github.com/Mattzeg/S7Dashboard.git
cd S7Dashboard
dotnet run
```

## Konfiguration

### SPS-Verbindung
| Parameter | Beschreibung | Standard |
|-----------|--------------|----------|
| IP-Adresse | IP der S7-1500 | 192.168.0.1 |
| Rack | Rack-Nummer | 0 |
| Slot | Slot-Nummer | 1 |
| Polling-Intervall | Abfrageintervall in ms | 1000 |

### Datenpunkte
| Parameter | Beschreibung |
|-----------|--------------|
| Name | Anzeigename |
| DB | Datenbaustein-Nummer |
| Byte | Start-Byte im DB |
| Typ | Real, Int, DInt, Bool |
| Einheit | z.B. °C, bar, % |
| Skalierung | Multiplikator |
| Offset | Additionswert |
| Dezimalstellen | Nachkommastellen |

### Kacheln
| Parameter | Beschreibung |
|-----------|--------------|
| Datenpunkt | Zugeordneter Datenpunkt |
| Breite/Höhe | Grid-Größe (1-12) |
| Schriftgrößen | Titel und Wert in px |
| Hintergrund/Schrift | Farbauswahl |
| Grenzwert-Farben | Farbe je nach Wertbereich |

### Grenzwert-Farben
Wenn aktiviert, ändert sich die Hintergrundfarbe automatisch:
- **Unter Grenze**: Wert < untere Grenze
- **Im Bereich**: Wert zwischen den Grenzen
- **Über Grenze**: Wert > obere Grenze

## Technologie

- **Framework**: ASP.NET Core 10.0 mit Blazor Server
- **SPS-Kommunikation**: S7NetPlus
- **Echtzeit-Updates**: SignalR (in Blazor integriert)
- **Konfiguration**: JSON-Datei (config.json)

## Netzwerkzugriff

Das Dashboard ist von anderen Geräten im Netzwerk erreichbar unter:
```
http://[Server-IP]:5000
```

## Ursprung

Entwickelt für die Futtertrocknung Erkheim zur Anzeige der Stromaufnahme der Pressen auf mobilen Geräten.

## Autor

Erstellt mit Unterstützung von Claude (Anthropic)
