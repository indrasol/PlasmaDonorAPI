using NewPlasmaDonorsAPI.Dto;

namespace NewPlasmaDonorsAPI.utils
{
    public class TreeUtils
    {
        public List<ProfileDto> Process(List<ProfileDto> profileList)
        {
            return Process(profileList, null);
        }

        public List<ProfileDto> Process(List<ProfileDto> profileList, ProfileDto? root)
        {
            List<ProfileDto> parents = new List<ProfileDto>();

            if (root != null && NullUtils.IsValid(root.id))
            {
                parents.Add(root);
            }
            else
            {
                var pids = profileList.Select(p => p.id).ToList();
                Console.WriteLine($":: {string.Join(", ", pids)}");

                parents = profileList
                    .Where(p => p.influencedById == null || !pids.Contains(p.influencedById))
                    .ToList();
            }

            var parentIds = parents.Select(p => p.id).ToList();
            Console.WriteLine($":: parentId :: {string.Join(", ", parentIds)}");

            var profiles = new Dictionary<long, ProfileDto>();
            foreach (var profile in profileList)
            {
                if (!profiles.ContainsKey((long)profile.id))
                {
                    profiles[(long)profile.id] = profile;
                }
            }

            List<ProfileDto> parentProfiles = new List<ProfileDto>();
            foreach (var profile in parents)
            {
                ProcessForParent(profiles, profile);
                parentProfiles.Add(profile);
            }

            return parentProfiles;
        }

        //private void ProcessForParent(Dictionary<long, ProfileDto> profiles, ProfileDto topProfile)
        //{
        //    BuildHierarchy(topProfile, profiles);
        //}

        private void PrintSubOrdinates(ProfileDto topProfileDto, int tabLevel = 0)
        {
            Console.WriteLine(new string('\t', tabLevel) + "-" + topProfileDto.influencedById);

            foreach (var e in topProfileDto.children)
            {
                PrintSubOrdinates(e, tabLevel + 1);
            }
        }

        public List<ProfileDto> FindAllProfilesByInfId(long? infId, Dictionary<long, ProfileDto> profiles)
        {
            List<ProfileDto> sameInfProfiles = new List<ProfileDto>();

            foreach (var e in profiles.Values)
            {
                if (infId.HasValue && e.influencedById == infId.Value)
                {
                    sameInfProfiles.Add(e);
                }
            }

            return sameInfProfiles;
        }

        private void ProcessForParent(Dictionary<long, ProfileDto> profiles, ProfileDto topProfile)
        {
            if (topProfile == null || profiles == null) return;

            BuildHierarchy(topProfile, profiles);
        }
        private void BuildHierarchy(ProfileDto parent, Dictionary<long, ProfileDto> profiles)
        {
            if (parent == null || parent.id == null) return;

            // Find direct children of the current parent
            var children = profiles.Values
                .Where(p => p.influencedById == parent.id)
                .ToList();

            // Ensure parent's children list is initialized
            if (parent.children == null)
            {
                parent.children = new List<ProfileDto>();
            }

            // Add each child to the parent's children list and recurse
            foreach (var child in children)
            {
                parent.children.Add(child);
                BuildHierarchy(child, profiles); // Recursively add grandchildren
            }
        }

        //private void BuildHierarchy(ProfileDto parent, Dictionary<long, ProfileDto> profiles)
        //{
        //    // Find direct children of the current parent
        //    var children = profiles.Values
        //        .Where(p => p.influencedById == parent.id)
        //        .ToList();

        //    // Add each child to the parent's children list and recurse
        //    foreach (var child in children)
        //    {
        //        parent.children.Add(child);
        //        BuildHierarchy(child, profiles); // Recursively find grandchildren, etc.
        //    }
        //}




        //private void BuildHierarchy(ProfileDto topProfileDto, Dictionary<long, ProfileDto> profiles)
        //{
        //    if (topProfileDto == null)
        //    {
        //        return;
        //    }

        //    var profileDtos = FindAllProfilesByInfId(topProfileDto.id, profiles);
        //    topProfileDto.children = profileDtos;

        //    if (profileDtos.Count == 0)
        //    {
        //        return;
        //    }

        //    foreach (var e in profileDtos)
        //    {
        //        BuildHierarchy(e, profiles);
        //    }
        //}
    }
}
