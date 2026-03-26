################################################################################
# Staging Override - US East 1
#
# This file provides region-specific overrides for staging.
# Use: terraform apply -var-file="staging.us-east-1.tfvars"
#
# NOTE: For database credentials, use AWS Secrets Manager (managed password)
# or set via environment variable: TF_VAR_db_password="..."
# NEVER commit plaintext passwords to version control.
################################################################################

environment = "staging"
region      = "us-east-1"

vpc_cidr_block = "10.20.0.0/16"

public_subnets = {
  a = {
    cidr_block = "10.20.0.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.20.1.0/24"
    az         = "us-east-1b"
  }
}

private_subnets = {
  a = {
    cidr_block = "10.20.10.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.20.11.0/24"
    az         = "us-east-1b"
  }
}

app_s3_bucket_name = "fsh-app-staging-us-east-1"

db_name     = "fshdb"
db_username = "fshadmin"

# Use managed password (recommended)
db_manage_master_user_password = true

container_image_tag = "staging"

api_cpu             = "256"
api_memory          = "512"
api_desired_count   = 2

blazor_cpu             = "256"
blazor_memory          = "512"
blazor_desired_count   = 2
