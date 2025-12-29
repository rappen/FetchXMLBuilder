using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Constants;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Rappen.XTB.FetchXmlBuilder
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "FetchXML Builder"),
        ExportMetadata("Description", "Build queries for Microsoft Dataverse. Run them. Get code. Let AI fix what you can't. Empower yourself to achieve more."),
        ExportMetadata("SmallImageBase64", "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"),
        ExportMetadata("BigImageBase64", "iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAIAAAABc2X6AAAABnRSTlMAAAA8ALSP22zwAAAACXBIWXMAAArrAAAK6wGCiw1aAAAQaklEQVR4nOVceXQUVbr/VVV3pbvT6c6+E0NYDIhDxlEWHcIDx8c+B2QcZ0BGkHGDh4BEFFlkgAdPAU/YFAV8Ir6jMzgRB0UHIZCIE1AROD4kEhHMvpG10+mu7qp6f9TS1d1Vle5O4ptz5vdHn6q63617f32/+917v/vdIvDLD/GvBEP/vp7gQfAAD4IDCUC49QdPAAQ4AAR4EjwBnui/GvUPYZIDwYHkVOgFQ/hHKOGGBQCeAEeAp/qDfJ8SJniQLCgOCIGn/nsoHuAAgCXBU+DIPqkg+owwyYFiQXAqSQQFkx1RdtpggdECgwkgYIgCCLBu8DxYN1g3w3SB6YS7FV63X3aKAzjwBFiBdm8bvNeESQ6UV0V1zfG0JQWWZETFgNBoH4NJvqTlK8YJVzPT1YiuBvCs+JDgYfACgNcAjupNfXtBWKhEQKvS0bTtFsRkKsmEB9oCOou2ZYFn4ahnOqvQ1aiorxc8C9YQsZJHSphkxb9chjmBjhuM6OTea50IgkJMBh2TAXcn034NHdXgOUD4oz1gSXCGCEwaEfY4HNywJhudMByW5HDLDg+Mk2ktR0e1sirwht3UYbYwwcHo9RlhykAn3gZbVp+1qg5oC51yB+wDmYZLYDoAAEJTU2DDYEEha06osiQLo0KNLcl0xhiYE38KtjIMZsqexYJAd4tUKx7gwYdqyUL+bygvKMlmgqATchE/+CelKoMg6YRbYUlk6r8SxzCKA+mBxxBKfULrAEq2lIHOGI34If8/bGWYE+gB42GKFW8D+po2QiBMsgq2NJ0xrt/tU4gwmOjMe2BJEm9Fzj2gJ8Ik5xt+jBY6Kx9RMb2pZB+DoOj00bBlSLccKI9+Dl3CwogngKLp9DEwWHpfyT4GQdLJd/jameJA6bWzjtFSsCUoOn0saKuWaI7VGW/uBuDyGP63zR5unXsLgqDTRzHVn8PVBgAUC44Er96W2oSNvhkynXoHTHo0StdOSbL/Q/mksTV/0otF30ZKPt3kOvXcnOzUo8qHx77YPOuNZ9QzEBSdNoqpKoXXJVbeY1Sdh2moNKlY+sQNhjVNv34kEahFyXGle+dv0s+lgwPztwawBWAxdenlMZjo1DuksYMPnPlKUCNM8DBIZtkUSycMC7O2IkbnFj6fXxxBxlX5pybmbYikSHMi4oeK14IHIghqhEl5QCPo5J+DiHy8XXH/0pHxrWFlGZ3ctPL+JyMukY4fCpNNvFGzXkGECR6U9MfED+3lIGQ1le/6w+bQ5SnwexastpiuRV4kQdBJI0XFJvhgzkGESUnCEEXHDQqxlOZ2TcmwFHvT5A9vH/jfWqlOV3RIbzHFwZYpXgf5m/wJK5qXTsgFGepMe3PRKo7XHLRWzHoqFMW+J7Vh0dRlWqksZ99X/NsQ60Mn5EpuFl6xBAACCZO+SRViBoT4dgDvXhn27ultWqlW83c9KjYFfsfDa010pZbA28Xbjt24JdQKGcywS8KUn+lSElY0b1yOpiNKA0vem19R84BWao+Kra/M12pnrzzyUFj1oe05viFKYa4VrOSnBImYTIQJB0st3r/dxWRrCayYtXRkfJtqkr4yuzxZ//HG9jZvmL4KOhrR8nzTp9UKF4/RI042bFl0St70nOu5qdUAatoSWX83SqK13UK7AcRFt5tod4K1xUS7Em0NB4rnZic2rJ97n1YdzpYvG1+4NeAhBf7cs0/ePvCAVq4XDx8dmlGRP+KwDrt2Z2qHM/FG/a3nKn7xxpdjxH+nq56p/UKUYGihweW/jZenVnRMOoBlU/ePG/GSThnBqKgb9mTR3LuGbJk2apWqwJjcwufzp2wunah8uGnyhzpsiy+uXXdy8qklJQm2Mp2iE2wAkJeDmXdj1QO3Hzi+/rlPfg1LEigjWA8AkJzg35WajvS5qWBODJFhAOKtzQAW/c+SysZpWjIrZvlNRfSVubp50sKDK8Oths3yzfKZs489sQkEhegU8anUnFILE5KWm1MEc9XYlh5uSRkJNwDUu6MW79/112cu0MbaYBmruXznvM3jd2wFYKXYXfNXa1lmtydzyf49td0mAC6PWX7u8mS9XbyhsilT6Gi0wZMS25ydfH10blGc9WtZ7N68P62bMHrD34YzgqOT4oR9K4mw1MK0JUG1+IMn9l2tDZxdJNpajQYPRbBJ9iYAlc1ZwvPjlQO2Fu1f/eBU1VeNGVa4Kn/KltKJm6e/d1v2QVUZAIVHXpHHoQ5nrPy8qXXE4vfnBstbqYKdv3lz7oQn5Cd/mLB7w6d/9kkQPHhCIKzYxTTFqRb/2ukZ55vV/wtVbDh135hb192rsQYouH+p3fKfj05+QjUVwImvX1h3copqUodTvcc5WOqRPy+8bcCFvMGvCU8yk04MT3RfrLbA4wREwiSg9MYRoMXJs8sT6V6JhEfeXFndPEk1yWoqXz5zNkk4VFMrG6cteNOv6zJeWlUyGCe/mSxfEwSTm9iEKGlNTnAQjZa89KV9G1+tXT4tigz1rqglB3YznvBsgcuT9fjrexoZP4Y3O+NDzO7o9nNCtbnMoKXFE8lDIizps7GPXVbHrme/emxHWFlePLyvuDqjZzkN5A//TL5u6byrrDaNpmVSMmH0F2EAKz+eea58aYjCJy68EDBKh44ca9ehOXsn5PncLO+WLO/mSFCShSeAQJ8WaZQvnS4/8onRXfA3WncmNQOw0kxyTCeA2rbYM/UpUMPig2uK15y2RV/Sr3FN86TH3ypQTapr9b05N+uDc8+YmtrT27tE+xob3ZKZVDE4/biBapbFyqvmvvDJLACgZFI8RMLS7IM2+HpOu9NvuffO8qkuJh6APbpC+V4ZJy++MHXvGtXqftMau7Xo5Y3z7tWiCoDl7E+/WVjd3bOlpMj2vEGv68t8frlg4RtrO4QJpo8woOXLDEa06bsEW1mCrUyVLYCWTr1B66XP8y/feFhH4K9nNh35fnCIlekRybGVw5KbVJP6LFhEH1aKM0d16ghkJ1f0YXFDMv7yztMTZw9ReWeoay4vm9je5TfTIglvXMx5+dZu0fNpbJxSlJNWpCMwKnfnqvzpW0on9FiT6qbJi/fvqWy3K53eY1MbRmZU/fqujyeMfFkY3k3GypcXLP503bEOf6+WH2HG69Ia4H+7reyj69k91kYV0wbe+OOkFT2KFcx66vi3Jeebexhy27uSP/kxK+BhWX1KWX3K3vN3bp708xWzZgkPU+NPLRp75r8+HakQFKYZnDTVYhk5xW7xmwbVd0bovkw1uQsXLFNdSATAai7f+8hqs5ozOS2uIcTi1h6f1t6VJ9/+24gSBSkCgX1Y2KcAAFhMzhDL0Mfu3+/JSv4oROGf5ezfMu293hTH8kRj2xD5NtleBVYK/OJlwrKtZjTtillj50IfT489M2P0s2FleWzKot8MvRpBWTJsFl+Yk9tjYRhJVTlAIiyptMcJTiRmM3co35JuV3dH6WB8Wt3aBx8LNxdFtm9fsGhgtJ9+CcvPUDAxsyYlrkS+rWwaJEXAQKHSyl02t5gcHdUrlU6Ncu/54wqLSX2w+aHu/iWvndVyZafGleyb7+fWNZA+L1xsTL1Woekm17Z565RPPrsyRtxDhajIBt+NsGZytcAc6tJEB6/M3TUkQ93t5vZkPrGvsKQ27ZefbXkwf4mqzLjbX9wyefSqT2YEJ9ktlWvGn2jqjGuV5oIGis1OqB+eeXVi3qEE2xeyZGPruLe+uBVsqXjPK514HCEE8DLOJjpuMABLlN/eZIotDJVeNe60lh8PwK6ju0pq0wAsO7zw7tyPByQfUxVbPO2pz8p/Jjg9Emy+bmk1l6/9vbpvIAAbD2/t6JBmB1IksmSuZLvVfVPoxlZzuzIzbeghdkLGv2dVPffAo1qpF689vvr4dOG6xWNceWi7l1X3YEQZq3ctXJxucgGgCFZVRgc7Pjj8+te/YOQ4TWnolXjKnmeeQ5dmJ+kRqSb3q48+aaJvqKY6unMfO7BR+aSoYvChYk1ncGbS3199qDDcOtQ0T1q6r2zlxzPBMXBK1k6KXFPMtKRuzHTW0DGZjW3p9S0T2rrSLHRHtPlmm1Nzr0yJ1x4qzEz6u1bq9vd3XGoJ9JkVHJkzfsTRnLT3VbNMvnNtwbf3vF364NfX72RZqrFdfYmSFt8IoNVh/7Ymu6hCWoQ46sV4VPhaVLHzQHJyFAs98FcRBOw8N+70n7S3Hc6VL8sP2nYQMG3gjb8UjNVahzndg6ZvKflcY7GtA6aqBK52AGBJsOIiUTHTUsSbM20/hvv2cWl1z85epJXq6M5d8tbzWqkfXc/WUWxL1LXdC1apTjn10N0isgXA+RTZf2rJSrcd18ExCBlmitu5YLXWqAtg14fbgpVZiYIjc36om6WVOvyWQy/N0NtbCgbT+r14JZyUkeBPWI6uZ71M6w+hv33jpA+GZx3SSr147bH1J9X9tTIcLFXw1kteVnMKsOC+p+/Lqgq1Qq42n+ll/Tj6ExbOUgho/V65ltDH78Zr2tJud85TB9eH8pKPrmcfKtbcVTcaGp+d0YNnRwbTfFm84omAMxJBHg85meeYpsuBqRoIjtOSsfto4bnGJK3UABQcmXO1+ndaqfG2nteYANBZg+6b4nVQ7HgQYZ6AvPXsqIGzMVAgHJwrX7bmREizIrFAllp+cAvjDXsfzwfOo2heMviAgJpPi6PkXs40XFB6BcJCmyNPxzJr4URV5s6/7YmsRABMwyVfT1QLGtBw4nkl16bXzTRciOyk2fp3XtG3zFpYfXx62ZXlEWRE+w04JLVnKdVYSw0nnmC9hNCIrgampYKWQ/rUUHxpXmqsn//t7NW7Xz1/VySVBgA8vG/9K/NsUQY/q/nVtVF6eVxtPqPDE1onP3SP8chRH0JAbfiRLj8dvE6mqhReqfd5aK0jTbp+aa/v2ARTf6GXBqwfwbqZ6n/42HrVA4cF6BLmCXh8GzNM7Zf/jJy9Lqb2rLjlDcDbw1HUnnYeeMJnwHiWqTnnswr/DGCcTPUZ35yZpZTTZlWEsNXCkT7O4Jm6r5iWq709IdwncDYz1aW+tmXJUI6ohba35McZuFnO1H0Z1uqi79F6jakp880RWEpeAOoj5M00joSH9oWDOOqZH0v8Drr+ZPA6mZoypvmyT8u8htCPH4YTwCjYMPloqbebqT0LexYdnxv5aeGwwHPo+JG5eQWsYuruNYZ1wDTMiE2Bs/JkXnsl46hF7CA6dlDo8dWRwFHH3LwCRrHjxZPwhn2EOKIqsgZwFAweMRqG9eLmd0zbNdgH0fZb+ri1eQ6dNUxrhR9VRH42PvwD00r4HTmVXmhNoWMGIDoZRK8O68PVynTWoLM6cPUS6dlwAb1TQqGpSa8i6pyHo55x1IOgYEmio5MRFaf33YMAME64W5nuZnQ1qLgfItLhAPS61/EEWCO4oC948Cy66hnBz0KQiLLCYIUxmjZGAQQoEwBwDHiO8brBusA4wDg0l6K9a1Ul+sjMCKsTVuODHjwHVwfQASC8sVtYtPl74XqJvrarHAmOFD/BQ7Ag1T6+ow/xuzRk3/KU0T8DibhzRYKV4hoJTvzmTnBfFj87RIi//fnlIfT715YgbT7LHyUIe1esj/F/HR0IjasHvOsAAAAASUVORK5CYII="),
        ExportMetadata("BackgroundColor", "#FFFFC0"),
        ExportMetadata("PrimaryFontColor", "#0000C0"),
        ExportMetadata("SecondaryFontColor", "#0000FF")]
    public partial class FetchXMLBuilderPlugin : PluginBase, IPayPalPlugin
    {
        public string DonationDescription => "FetchXML Builder Fan Club";

        public string EmailAccount => "jonas@rappen.net";

        public override IXrmToolBoxPluginControl GetControl()
        {
            var tool = new FetchXmlBuilder();
            tool.SaveImageFromBase64(
                tool.IconBigPath,
                GetType()
                .GetCustomAttributes(typeof(ExportMetadataAttribute), false)
                .OfType<ExportMetadataAttribute>()
                .FirstOrDefault(a => a.Name == "BigImageBase64")?.Value as string);
            tool.SaveImageFromBase64(
                tool.IconSmallPath,
                GetType()
                .GetCustomAttributes(typeof(ExportMetadataAttribute), false)
                .OfType<ExportMetadataAttribute>()
                .FirstOrDefault(a => a.Name == "SmallImageBase64")?.Value as string);
            return tool;
        }

        public override Guid GetId() => XrmToolBoxToolIds.FetchXMLBuilder;

        public FetchXMLBuilderPlugin()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var currAssembly = Assembly.GetExecutingAssembly();
            var argName = args.Name.Split(',')[0];
            if (currAssembly.GetReferencedAssemblies().Any(a => a.Name == argName))
            {
                return Assembly.LoadFrom(Path.Combine(Paths.PluginsPath, Path.GetFileNameWithoutExtension(currAssembly.Location), $"{argName}.dll"));
            }
            return null;
        }
    }
}