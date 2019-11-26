using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;

namespace Gradual_Romance
{
    class JobDriver_AskForLovin : JobDriver
    {
        private const TargetIndex PersonToAsk = TargetIndex.A;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(PersonToAsk), this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(PersonToAsk);
            this.FailOnDowned(PersonToAsk);
            this.FailOnNotCasualInterruptible(PersonToAsk);

            yield return Toils_Goto.GotoThing(PersonToAsk, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(this.pawn);
            yield return Toils_Interpersonal.GotoInteractablePosition(PersonToAsk);

            //change for our new interaction
            Toil gotoTarget = Toils_Interpersonal.Interact(PersonToAsk, InteractionDefOf.Chitchat);

        }
    }
}
