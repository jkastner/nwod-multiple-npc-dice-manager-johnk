using GameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace XMLCharSheets
{
    public class MoveAndMeleeAttackScript : CharacterActionScript
    {
        CharacterSheet _activeChar = null;
        public override void PerformAction(CharacterSheet activeCharacter)
        {
            _activeChar = activeCharacter;
            if (_activeChar.IsIncapacitated)
            {
                TextReporter.Report(_activeChar.Name + " is incapacitated and cannot act.\n"); 
                return;
            }
            //1 Find target
            CharacterSheet validTarget = _activeChar.Target;
            if (validTarget == null || validTarget.IsIncapacitated)
            {
                validTarget = FindValidTarget();
                if (validTarget == null)
                {
                    TextReporter.Report(_activeChar.Name + " could not find a valid target and cannot act.\n");
                    return;
                }
                TextReporter.Report(_activeChar.Name + " changes to target " + validTarget.Name+"\n");
            }

            bool canAttack = true;
            //2 Move towards target, if necessary.
            if (VisualsService.BoardsViewModel.HasAssociatedVisual(_activeChar.UniqueCharacterID) && VisualsService.BoardsViewModel.HasAssociatedVisual(validTarget.UniqueCharacterID))
            {
                canAttack = MoveToTarget(validTarget);
            }
            if (canAttack)
            {
                //3 Select attack
                //internal void SetTarget(CharacterSheet target, String attackType, string damageType)
                List<AttackTrait> possibleAttacks = _activeChar.NumericTraits.Where(x => x is AttackTrait).Select(x => x as AttackTrait).ToList();
                if (possibleAttacks.Count == 0)
                {
                    TextReporter.Report(_activeChar.Name + " has no valid attacks.\n");
                }
                var chosenAttack = possibleAttacks.Aggregate((i1, i2) => i1.TraitValue > i2.TraitValue ? i1 : i2);

                _activeChar.SetTarget(validTarget, new List<string>(), chosenAttack.TraitLabel, chosenAttack.DamageType);
                CombatService.RosterViewModel.RollAttackTarget(new List<CharacterSheet>(){_activeChar});
                //4 Clean deceased characters.
                CombatService.RosterViewModel.MarkCharactersAsDeceased();
            }
            
        }

        protected virtual bool MoveToTarget(CharacterSheet target)
        {
            bool canAttack = true;
            var neededDistance = _activeChar.DistanceTo(target.FirstVisual.LocationForSave);
            //1. Character doesn't need to move.
            //2. Character needs to move once.
            //3. Character needs to move twice, and hence cannot attack.

            if (neededDistance <= _activeChar.HeightTrait.TraitValue)
            {
                return canAttack;
            }
            neededDistance -= _activeChar.HeightTrait.TraitValue;

            if (neededDistance > _activeChar.SpeedTrait.TraitValue)
            {
                if(neededDistance > _activeChar.SpeedTrait.TraitValue * 2)
                {
                    neededDistance = _activeChar.SpeedTrait.TraitValue * 2;
                }
                canAttack = false;
            }
            var targetPoint = Helper3DCalcs.MovePointTowards(_activeChar.FirstVisual.LocationForSave, target.FirstVisual.LocationForSave, neededDistance);
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.MovePieceToPoint(_activeChar.UniqueCharacterID, targetPoint));
            return canAttack;
        }

        protected virtual CharacterSheet FindValidTarget()
        {
            CharacterSheet validTarget = null;
            double distance = double.MaxValue;
            //Loop through each non-team member, look for closest.
            //1. This has no visual and hence no team.
            //2. This has a visual, no opponent has a visual.
            //3. This has a visual, opponents also have a visual.
            
            //Case 1. This has no visual.
            if (_activeChar.FirstVisual==null)
            {
                validTarget = CombatService.RosterViewModel.ActiveRoster.FirstOrDefault
                    (x => x != _activeChar && !x.IsIncapacitated);
            }
            //Cases 2 and 3
            else
            {
                
                var possibleTargetsWithVisual = CombatService.RosterViewModel.ActiveRoster.Where(x => x!=_activeChar 
                    && x.Team != _activeChar.Team && x.HasVisual && !x.IsIncapacitated).ToList();
                var possibleTargetsWithoutVisual = CombatService.RosterViewModel.ActiveRoster.Where(x =>  x!=_activeChar 
                    && x.Team != _activeChar.Team && !x.IsIncapacitated && !x.HasVisual).ToList();
                //Case 2. This has a visual, no opponent has a visual.
                if (possibleTargetsWithVisual.Count() == 0)
                {
                    validTarget = possibleTargetsWithoutVisual.FirstOrDefault();
                }
                else
                {
                    foreach (var cur in possibleTargetsWithVisual)
                    {
                        Point3D curLoc = cur.FirstVisual.LocationForSave;
                        double curDistance = _activeChar.DistanceTo(curLoc);
                        if (curDistance < distance)
                        {
                            distance = curDistance;
                            validTarget = cur;
                        }
                    }
                }
            }
            return validTarget;
        }

    }
}
