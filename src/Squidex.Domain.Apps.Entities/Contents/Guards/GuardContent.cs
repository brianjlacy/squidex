﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using NodaTime;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Entities.Contents.Guards
{
    public static class GuardContent
    {
        public static void CanCreate(CreateContent command)
        {
            Guard.NotNull(command, nameof(command));

            Validate.It(() => "Cannot created content.", e =>
            {
                ValidateData(command, e);
            });
        }

        public static void CanUpdate(UpdateContent command)
        {
            Guard.NotNull(command, nameof(command));

            Validate.It(() => "Cannot update content.", e =>
            {
                ValidateData(command, e);
            });
        }

        public static void CanPatch(PatchContent command)
        {
            Guard.NotNull(command, nameof(command));

            Validate.It(() => "Cannot patch content.", e =>
            {
                ValidateData(command, e);
            });
        }

        public static void CanDiscardChanges(bool isPending, DiscardChanges command)
        {
            Guard.NotNull(command, nameof(command));

            if (!isPending)
            {
                throw new DomainException("The content has no pending changes.");
            }
        }

        public static void CanChangeContentStatus(bool isPending, Status status, ChangeContentStatus command)
        {
            Guard.NotNull(command, nameof(command));

            Validate.It(() => "Cannot change status.", e =>
            {
                if (!StatusFlow.Exists(command.Status))
                {
                    e("Status is not valid.", nameof(command.Status));
                }
                else if (!StatusFlow.CanChange(status, command.Status))
                {
                    if (status == command.Status && status == Status.Published)
                    {
                        if (!isPending)
                        {
                            e("Content has no changes to publish.", nameof(command.Status));
                        }
                    }
                    else
                    {
                        e($"Cannot change status from {status} to {command.Status}.", nameof(command.Status));
                    }
                }

                if (command.DueTime.HasValue && command.DueTime.Value < SystemClock.Instance.GetCurrentInstant())
                {
                    e("Due time must be in the future.", nameof(command.DueTime));
                }
            });
        }

        public static void CanDelete(DeleteContent command)
        {
            Guard.NotNull(command, nameof(command));
        }

        private static void ValidateData(ContentDataCommand command, AddValidation e)
        {
            if (command.Data == null)
            {
                e("Data is required.", nameof(command.Data));
            }
        }
    }
}
