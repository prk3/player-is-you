using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using Traits;

/**
 * Loads map from level_.txt file, renders tiles, parses and applies rules.
 * IMPORTANT: tiles exist in a plane where Y axis grows downwards (like in images)
 */
public class Map : MonoBehaviour
{
    public int levelId;
    public int width;
    public int height;
    public List<Entity>[][] stacks;
    private List<(EntityType, EntityType)> _prevRules;

    private ObjectFitContain _contain;

    void Start()
    {
        string levelFile = $"Levels/{levelId}";
        TextAsset text = Resources.Load<TextAsset>(levelFile);
        string[] lines = text.text.Split('\n');

        width = int.Parse(lines[0]);
        height = int.Parse(lines[1]);

        var mapBackground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mapBackground.name = "background";
        mapBackground.transform.parent = gameObject.transform;
        mapBackground.transform.localPosition = new Vector3(width / 2.0f, -height / 2.0f, 1);

        mapBackground.transform.localScale = new Vector3(width / 10.0f, 1, height / 10.0f);

        var rotation = mapBackground.transform.localRotation.eulerAngles;
        rotation.x = -90;
        mapBackground.transform.localRotation = Quaternion.Euler(rotation);

        mapBackground.GetComponent<MeshRenderer>().material.color = new Color(0.08f, 0.08f, 0.08f, 1f);
        mapBackground.GetComponent<MeshRenderer>().material.shader = Shader.Find("UI/Default");

        stacks = new List<Entity>[height][];

        for (int y = 0; y < height; y++)
        {
            stacks[y] = new List<Entity>[width];
            for (int x = 0; x < width; x++)
            {
                stacks[y][x] = new List<Entity>(2); // entities rarely stack in more than 2
                var id = int.Parse(lines[2].Substring((width * y + x) * 2, 2));

                // we skip id 0, which encodes an empty tile
                if (id == 0) continue;

                EntityType type;

                try
                {
                    type = (EntityType) id;
                }
                catch (InvalidCastException)
                {
                    Debug.LogWarning($"Invalid entity type {id} in map file {levelFile}");
                    continue;
                }

                var tile = MakeEntityObject(x, y, type);
                stacks[y][x].Add(tile.GetComponent<Entity>());
            }
        }

        _prevRules = new List<(EntityType, EntityType)>();
        UpdateRules();

        _contain = gameObject.AddComponent<ObjectFitContain>();
    }

    void Update()
    {
        if (_contain.Contain(width, height))
        {
            // invert Y transform
            var localPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(localPos.x, -localPos.y, localPos.z);
        }
    }

    private GameObject MakeEntityObject(int x, int y, EntityType type)
    {
        var obj = new GameObject("entity");
        obj.transform.parent = gameObject.transform;

        var entity = obj.AddComponent<Entity>();
        entity.SetEntityType(type);
        entity.x = x;
        entity.y = y;
        entity.z = 0;

        if (Entity.IsSubject(type) || Entity.IsTrait(type) || type == EntityType.ConnectorIs)
        {
            obj.AddComponent<Traits.Push>();
        }

        var ren = obj.AddComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(
            Entity.GetTexture(),
            new Rect(32 * (int) (type-1), 0, 32, 32),
            new Vector2(0, 0),
            32);

        obj.AddComponent<AnimatedSprite>();

        var mod = Entity.GetModTilemap(type);

        if (mod != null)
        {
            var modComp = obj.AddComponent<TileMod>();
            modComp.modTilemap = mod;
        }

        return obj;
    }

    public bool IsValidSpot(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public byte CollectNeighborsByte(Entity e, bool bitForMissingNeighbors = false)
    {

        EntityType type = e.GetEntityType();

        bool CollectStack(int x, int y)
        {
            if (!IsValidSpot(new Vector2Int(x, y))) return bitForMissingNeighbors;
            return stacks[y][x].Find(entity => entity.GetEntityType() == type);
        }

        byte output = 0;

        output |= (byte)(CollectStack(e.x - 1, e.y - 1) ?   1 : 0);
        output |= (byte)(CollectStack(e.x + 0, e.y - 1) ?   2 : 0);
        output |= (byte)(CollectStack(e.x + 1, e.y - 1) ?   4 : 0);

        output |= (byte)(CollectStack(e.x - 1, e.y + 0) ?   8 : 0);

        output |= (byte)(CollectStack(e.x + 1, e.y + 0) ?  16 : 0);

        output |= (byte)(CollectStack(e.x - 1, e.y + 1) ?  32 : 0);
        output |= (byte)(CollectStack(e.x + 0, e.y + 1) ?  64 : 0);
        output |= (byte)(CollectStack(e.x + 1, e.y + 1) ? 128 : 0);

        return output;
    }

    private List<(EntityType, EntityType)> ExtractRules()
    {
        var output = new List<(EntityType, EntityType)>();

        foreach (var row in stacks)
        {
            foreach (var stack in row)
            {
                foreach (Entity entity in stack)
                {
                    if (entity.GetEntityType() == EntityType.ConnectorIs)
                    {
                        ExtractRulesFromConnectorIs(entity, output);
                    }
                }
            }
        }

        return output;
    }

    private void ExtractRulesFromConnectorIs(Entity entity, List<(EntityType, EntityType)> list)
    {
        // is connector can be a part of a vertical rule
        if (entity.y != 0 && entity.y != height - 1)
        {
            ExtractRulesFromSpots(stacks[entity.y - 1][entity.x], stacks[entity.y + 1][entity.x], list);
        }

        // is connector can be a part of a horizontal rule
        if (entity.x != 0 && entity.x != width - 1)
        {
            ExtractRulesFromSpots(stacks[entity.y][entity.x - 1], stacks[entity.y][entity.x + 1], list);
        }
    }

    private void ExtractRulesFromSpots(List<Entity> primaryStack, List<Entity> secondaryStack, List<(EntityType, EntityType)> list)
    {
        foreach (var primary in primaryStack)
        {
            var primaryType = primary.GetEntityType();
            var isPrimarySubject = Entity.IsSubject(primaryType);
            var isPrimaryTrait = Entity.IsTrait(primaryType);

            if (!isPrimarySubject && !isPrimaryTrait) continue;

            foreach (var secondary in secondaryStack)
            {
                var secondaryType = secondary.GetEntityType();
                var isSecondarySubject = Entity.IsSubject(secondaryType);
                var isSecondaryTrait = Entity.IsTrait(secondaryType);

                if (isPrimarySubject && isSecondaryTrait)
                {
                    list.Add((primaryType, secondaryType));
                }

                if (isSecondarySubject && isPrimaryTrait)
                {
                    list.Add((secondaryType, primaryType));
                }
            }
        }
    }

    public void UpdateRules()
    {
        var newRules = ExtractRules();
        var addedRules = newRules.Except(_prevRules);
        var deletedRules = _prevRules.Except(newRules);

        foreach (var (subject, trait) in deletedRules)
        {
            var targetType = Entity.GetEntityTypeFromSubject(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var s in entities)
            {
                if (s.GetEntityType() != targetType) continue;

                var component = s.gameObject.GetComponent(Entity.GetBehaviorFromTrait(trait));
                if (component) Destroy(component);
            }
        }

        foreach (var (subject, trait) in addedRules)
        {
            var targetType = Entity.GetEntityTypeFromSubject(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var entity in entities)
            {
                if (entity.GetEntityType() != targetType) continue;

                entity.gameObject.AddComponent(Entity.GetBehaviorFromTrait(trait));
            }
        }

        _prevRules = newRules;
    }

    public void RefreshPositions(List<Vector2Int> positions)
    {
        var affectedTiles = new HashSet<(int, int)>();

        void TryAddPosition(Vector2Int pos)
        {
            if (IsValidSpot(pos)) affectedTiles.Add((pos.x, pos.y));
        }

        foreach (var position in positions)
        {
            TryAddPosition(position + Vector2Int.left   + Vector2Int.down);
            TryAddPosition(position                     + Vector2Int.down);
            TryAddPosition(position + Vector2Int.right  + Vector2Int.down);

            TryAddPosition(position + Vector2Int.left);
            TryAddPosition(position);
            TryAddPosition(position + Vector2Int.right);

            TryAddPosition(position + Vector2Int.left   + Vector2Int.up);
            TryAddPosition(position                     + Vector2Int.up);
            TryAddPosition(position + Vector2Int.right  + Vector2Int.up);
        }

        foreach (var (x, y) in affectedTiles)
        {
            foreach (var entity in stacks[y][x])
            {
                entity.Refresh();
            }
        }
    }

    public void ApplyRules() {
        for (int y = 0; y < height; y++)
        {
            var row = stacks[y];
            for (int x = 0; x < width; x++)
            {
                var stack = row[x];

                for (int t = 0; t < 20; t++)
                {
                    if (ApplyRulesOnStack(stack) != RuleApplicationOutcome.Refresh) goto triesEnd;
                }

                Debug.LogError($"The limit of trait updates has been reached for stack x={x}, y={y}");

                triesEnd: ;
            }
        }
    }

    public RuleApplicationOutcome ApplyRulesOnStack(List<Entity> stack)
    {
        var traitOrderPairs = new List<(int, Trait)>();

        for (int i = 0; i < stack.Count; i++)
        {
            foreach (var trait in stack[i].GetComponents<Trait>())
            {
                traitOrderPairs.Add((trait.GetInteractionOrder() - i, trait));
            }
        }

        traitOrderPairs.Sort((a, b) => b.Item1 - a.Item1);

        foreach (var (_, trait) in traitOrderPairs)
        {
            switch (trait.ApplyRuleOnStack(stack))
            {
                case RuleApplicationOutcome.Refresh:
                    return RuleApplicationOutcome.Refresh;
                case RuleApplicationOutcome.Break:
                    return RuleApplicationOutcome.Break;
                case RuleApplicationOutcome.Continue:
                    break;
            }
        }
        return RuleApplicationOutcome.Break;
    }
}
