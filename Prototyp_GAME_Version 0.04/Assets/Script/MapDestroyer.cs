﻿using System.Collections;
using UnityEngine;

public class MapDestroyer : MonoBehaviour
{
    public LevelGenerator levelGenerator;
    public PlayerSpawner PlayerSpawner;
    public GameObject ExplosionPrefab;
    private IEnumerator coroutinexPositiv;
    private IEnumerator coroutinexNegativ;
    private IEnumerator coroutinezPositiv;
    private IEnumerator coroutinezNegativ;
    private Vector3 explosionPosition;

    //Wird beim explodieren der Bombe durch das BombeScript aufgerufen.
    //Empfaengt die Position, die bombPower der Bombe und die ID des Spielers welche sie gelegt hat.
    public void Explode(Vector3 position, int bombPower, int id)
    {
        explosionPosition = position;

        //Ueberprüft ob der Spieler, der die Bombe gelegt hat, exakt an der gleichen Stelle (in der Bombe) stehen geblieben ist.
        //Sollte dies zutreffen, wird der Spieler getoetet.
        if ((int) PlayerSpawner.playerList[id].gameObject.transform.position.x == (int)explosionPosition.x && (int) PlayerSpawner.playerList[id].gameObject.transform.position.z == (int)explosionPosition.z)
        {
             PlayerSpawner.playerList[id].GetComponent<PlayerScript>().dead(id);
        }

        //Die explodierte Bombe wird dem Spieler wieder gutgeschrieben.
        PlayerSpawner.playerList[id].GetComponent<PlayerScript>().setAvaibleBomb(1);

        //Explosions-Animation an der Stelle der Bombe wird abgespielt.
        Instantiate(ExplosionPrefab, explosionPosition, Quaternion.identity);

        //Es werden 4 Coroutinen angelegt und gestartet, welche gleichzeitig in alle Himmelsrichtung (x, -x, z, -z) die Fehler durchlaufen.
        //Die bombPower gibt an wieviele Felder in jede Richtung erreicht und geprueft werden muessen.
        //Die Player ID der Bombe (= ID vom Player der sie gelegt hat) wird durchgereicht, falls diese spaeter noch benoetigt wird.
        coroutinexPositiv = xPositiv(bombPower, 0.1f, id);
        coroutinexNegativ = xNegativ(bombPower, 0.1f, id);
        coroutinezPositiv = zPositiv(bombPower, 0.1f, id);
        coroutinezNegativ = zNegativ(bombPower, 0.1f, id);

        StartCoroutine(coroutinexPositiv);
        StartCoroutine(coroutinexNegativ);
        StartCoroutine(coroutinezPositiv);
        StartCoroutine(coroutinezNegativ);
    }

    //Jeder der 4 Coroutinen laeuft solange der Rueckgabewert von ExplodeCell = true ist.
    private IEnumerator xPositiv(int bombPower, float waitTime, int id)
    {
        yield return new WaitForSeconds(waitTime);

        int distanz = 1;
        while (distanz <= bombPower && ExplodeCell((int)explosionPosition.x + distanz, (int)explosionPosition.z, id))
        {
            yield return new WaitForSeconds(waitTime);
            distanz++;
        }
    }

    private IEnumerator xNegativ(int bombPower, float waitTime, int id)
    {
        yield return new WaitForSeconds(waitTime);

        int distanz = 1;
        while (distanz <= bombPower && ExplodeCell((int)explosionPosition.x - distanz, (int)explosionPosition.z, id))
        {
            yield return new WaitForSeconds(waitTime);
            distanz++;
        }
    }

    private IEnumerator zPositiv(int bombPower, float waitTime, int id)
    {
        yield return new WaitForSeconds(waitTime);

        int distanz = 1;
        while (distanz <= bombPower && ExplodeCell((int)explosionPosition.x, (int)explosionPosition.z + distanz, id))
        {
            yield return new WaitForSeconds(waitTime);
            distanz++;
        }
    }

    private IEnumerator zNegativ(int bombPower, float waitTime, int id)
    {
        yield return new WaitForSeconds(waitTime);

        int distanz = 1;
        while (distanz <= bombPower && ExplodeCell((int)explosionPosition.x, (int)explosionPosition.z - distanz, id))
        {
            yield return new WaitForSeconds(waitTime);
            distanz++;
        }
    }


    //Prueft was fuer ein GameObject sich in der zu pruefenden Zelle befindet und "reagiert" entsprechend.
    //Waende und Kisten sorgen dafuer das der Rueckgabewert = false wird und die Explosion in diese Richtung nicht mehr fortgesetzt wird.
    //Bomben stoppen die aktuelle Explosion ebenfalls, werden aber direkt gezuendet in dem ihr Timer auf 0 und die remoteBomb funktion deaktiviert wird.
    //So werden Kettenreaktionen unter den Bomben erzeugt.
    bool ExplodeCell(int x, int z, int id)
    {
        GameObject thisGameObject = levelGenerator.AllGameObjects[x, z];

        if (thisGameObject == null)
        {
            Instantiate(ExplosionPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
            return true;
        }
        else
        {
            //Switch-Case prueft den Tag des GameObjects. Performance ist so besser als mit dem Namen zu arbeiten, welcher ein Sting waere. 
            switch (thisGameObject.tag)
            {
                case "Bombe":
                    Instantiate(ExplosionPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    thisGameObject.GetComponent<BombScript>().bombTimer = 0;
                    thisGameObject.GetComponent<BombScript>().remoteBomb = false;
                    return false;

                case "Wand":
                    return false;

                case "Kiste":
                    Instantiate(ExplosionPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    Destroy(thisGameObject);
                    return false;

                case "Item":
                    Instantiate(ExplosionPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    Destroy(thisGameObject);
                    return true;

                case "Player":
                    Instantiate(ExplosionPrefab, new Vector3(x, 0.5f, z), Quaternion.identity);
                    thisGameObject.GetComponent<PlayerScript>().dead(thisGameObject.GetComponent<PlayerScript>().getPlayerID());
                    levelGenerator.AllGameObjects[x, z] = null;
                    return true;

                default:
                    break;
            }
        }
        return false;
    }
}