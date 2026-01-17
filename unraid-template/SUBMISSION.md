# Submitting PlexBot to unRAID Community Applications

This guide explains how to submit PlexBot templates to the unRAID Community Applications repository.

## Prerequisites

- Templates are already created in this repository under `unraid-template/`
- Icon is available at the correct URL
- Docker images are published to GHCR (GitHub Container Registry)

## Submission Options

### Option 1: Submit to selfhosters/unRAID-CA-templates (Recommended)

The selfhosters repository is a well-known community repository that hosts many application templates.

#### Steps:

1. Fork the repository: https://github.com/selfhosters/unRAID-CA-templates

2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/unRAID-CA-templates.git
   cd unRAID-CA-templates
   ```

3. Copy the template files:
   ```bash
   cp /path/to/PlexBot/unraid-template/plexbot.xml templates/
   cp /path/to/PlexBot/unraid-template/plexbot-lavalink.xml templates/
   ```

4. Commit and push:
   ```bash
   git add templates/plexbot.xml templates/plexbot-lavalink.xml
   git commit -m "Add PlexBot and PlexBot-Lavalink templates"
   git push origin main
   ```

5. Create a Pull Request:
   - Go to your fork on GitHub
   - Click "Pull Request"
   - Provide a description explaining what PlexBot does
   - Wait for review

#### Important Notes for selfhosters repository:

From their README, they mention:
- The template must be made by a user with previous GitHub activity
- The application must be of certain quality
- Not fully AI written
- Be attributed to a GitHub account with an active history

### Option 2: Create Your Own Template Repository

You can host the templates directly in this repository and users can add it to their unRAID Community Applications.

#### Steps:

1. Templates are already in `unraid-template/` directory ✓
2. Icon is already in `Images/plexbot-unraid-icon.png` ✓
3. Ensure the repository is public ✓

Users can then add this repository to their unRAID Community Applications:

1. In unRAID, go to **Apps** → **App Install Options**
2. Add template repository URL:
   ```
   https://github.com/julesdg6/plexbot
   ```
3. The templates will appear in the Apps page

### Option 3: Direct Template URL

Users can install PlexBot directly using the template URL without adding to Community Applications:

1. In unRAID Docker tab, click **Add Container**
2. Click **Template** dropdown
3. At the bottom, paste template URL:
   ```
   https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-lavalink.xml
   ```
4. Configure and apply
5. Repeat for PlexBot template:
   ```
   https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot.xml
   ```

## Verification Checklist

Before submitting, verify:

- [ ] Both XML templates are valid XML (no parsing errors)
- [ ] Icon URL is accessible: `https://raw.githubusercontent.com/julesdg6/plexbot/main/Images/plexbot-unraid-icon.png`
- [ ] Docker images are published and accessible:
  - [ ] `ghcr.io/julesdg6/plexbot:latest`
  - [ ] `ghcr.io/lavalink-devs/lavalink:4`
- [ ] All required environment variables are documented
- [ ] Templates include proper descriptions and categories
- [ ] Support links are correct
- [ ] Repository is public

## Docker Image Publishing

If the Docker images aren't published yet, you need to set up GitHub Actions to build and publish them:

1. Create `.github/workflows/docker-publish.yml`:

```yaml
name: Docker Build and Push

on:
  push:
    branches: [ main ]
    tags: [ 'v*.*.*' ]
  pull_request:
    branches: [ main ]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Log in to the Container registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}

      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./Install/Docker/dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
```

2. Commit and push this workflow to enable automatic Docker image builds

3. After the workflow runs successfully, your image will be available at:
   ```
   ghcr.io/julesdg6/plexbot:latest
   ```

## Testing Before Submission

Test the templates locally on an unRAID server:

1. Copy template XML files to unRAID at:
   ```
   /boot/config/plugins/dockerMan/templates-user/
   ```

2. In unRAID web UI, go to Docker tab

3. Click **Add Container** and select your templates

4. Verify all configuration fields appear correctly

5. Test that the containers start and work properly

6. Test that environment variables are properly passed

7. Verify the icon displays correctly

## Common Issues

### Templates don't appear in unRAID

- Ensure XML files are valid
- Check file permissions (should be readable)
- Restart Docker service or refresh Apps page

### Icon doesn't display

- Verify the icon URL is publicly accessible
- Check the URL doesn't require authentication
- Icon should be PNG format, preferably 512x512 pixels

### Containers won't start

- Check all required environment variables are set
- Verify Docker images are accessible and not private
- Review container logs in unRAID

## Additional Resources

- [unRAID Docker Template XML Schema](https://forums.unraid.net/topic/38619-docker-template-xml-schema/)
- [unRAID Docker FAQ](https://forums.unraid.net/topic/57181-real-docker-faq/)
- [selfhosters Discord](https://discord.gg/qWPbc8R) - For help with template submissions
- [Community Applications Plugin](https://github.com/Squidly271/community.applications)

## After Submission

Once your templates are accepted:

1. Users can find PlexBot in the Apps tab by searching
2. Consider creating a support thread on unRAID forums
3. Update your main README to mention unRAID support
4. Monitor the repository for user feedback and issues

Remember to maintain the templates when you update the application:
- Update version tags if needed
- Keep documentation current
- Respond to user issues related to unRAID deployment
