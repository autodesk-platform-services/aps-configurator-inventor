# Windows build machine creation
Those files were used to create Docker image for the Windows build machine. They need to be altered and re-used when there is a change in the build environment (like upgrade of .Net Framework or VisualStudio)

Since UI tests run on this machine, we need working chromium to be installed and running there. The chromium installation is handled by npm module, but it does not run without several system fonts, which are not installed by default on windows server 2019. We need to install those fonts using dockerfile. Credists goes here https://github.com/prom3theu5/ServerCoreFonts

Since the tight relation between the machine where is the image created and where it is used, it is recomended to run this on the EC2 macine (with the same version) if Windows which is used by CodeBuild.
There might be a problem to log to Amazon ECR (docker registry) to push the resulting image. Following post helps with that: https://stackoverflow.com/questions/60807697/docker-login-error-storing-credentials-the-stub-received-bad-data