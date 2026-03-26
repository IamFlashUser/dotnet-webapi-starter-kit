################################################################################
# Production Override - US East 1
#
# This file provides region-specific overrides for production.
# Use: terraform apply -var-file="prod.us-east-1.tfvars"
#
# NOTE: For database credentials, use AWS Secrets Manager (managed password)
# or set via environment variable: TF_VAR_db_password="..."
# NEVER commit plaintext passwords to version control.
################################################################################

environment = "prod"
region      = "us-east-1"

vpc_cidr_block = "10.30.0.0/16"

public_subnets = {
  a = {
    cidr_block = "10.30.0.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.30.1.0/24"
    az         = "us-east-1b"
  }
  c = {
    cidr_block = "10.30.2.0/24"
    az         = "us-east-1c"
  }
}

private_subnets = {
  a = {
    cidr_block = "10.30.10.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.30.11.0/24"
    az         = "us-east-1b"
  }
  c = {
    cidr_block = "10.30.12.0/24"
    az         = "us-east-1c"
  }
}

app_s3_bucket_name = "fsh-app-prod-us-east-1"

db_name     = "fshdb"
db_username = "fshadmin"

# Use managed password (recommended for production)
db_manage_master_user_password = true

container_image_tag = "latest"

api_cpu             = "512"
api_memory          = "1024"
api_desired_count   = 3

blazor_cpu             = "512"
blazor_memory          = "1024"
blazor_desired_count   = 3
