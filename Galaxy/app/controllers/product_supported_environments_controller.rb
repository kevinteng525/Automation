class ProductSupportedEnvironmentsController < ApplicationController
  def index
    @supported_environments = Project.get_force_supported_environments_for_project(params['project_id'])
  end
end
