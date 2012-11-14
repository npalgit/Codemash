﻿using System;
using Codemash.Phone.Shared.Common;

namespace Codemash.Phone.Shared.Grouping
{
    public static class GroupingFactory
    {
        public static IGroup GetGroupInstance(SessionGroupType groupType)
        {
            switch (groupType)
            {
                case SessionGroupType.ByTech:
                    return new ByTechGroup();
                case SessionGroupType.ByBlock:
                    return new ByBlockGroup();
                case SessionGroupType.ByName:
                    return new ByNameGroup();
                case SessionGroupType.ByRoom:
                    return new ByRoomGroup();
            }

            throw new InvalidOperationException("Unable to determine grouping type");
        }
    }
}
