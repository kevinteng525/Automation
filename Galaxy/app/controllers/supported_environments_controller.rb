class SupportedEnvironmentsController < ApplicationController
  before_action :set_supported_environment, only: [:show, :edit, :update, :destroy]

  # GET /supported_environments
  # GET /supported_environments.json
  def index
    @supported_environments = SupportedEnvironment.get_all_force_supported_environments()
  end

  # GET /supported_environments/1
  # GET /supported_environments/1.json
  def show
  end

  # GET /supported_environments/new
  def new
    @supported_environment = SupportedEnvironment.new
  end

  # GET /supported_environments/1/edit
  def edit
    def @supported_environment.new_record?()
      false
    end 
  end

  # POST /supported_environments
  # POST /supported_environments.json
  def create
    @supported_environment = SupportedEnvironment.create_force_supported_environment(supported_environment_params)

    respond_to do |format|
      if @supported_environment
        format.html { redirect_to supported_environment_path(@supported_environment), notice: 'Supported environment was successfully created.' }
        format.json { render action: 'show', status: :created, location: @supported_environment }
      else
        format.html { render action: 'new' }
        format.json { render json: @supported_environment.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /supported_environments/1
  # PATCH/PUT /supported_environments/1.json
  def update
    @supported_environment = SupportedEnvironment.update_force_supported_environment(params[:id], supported_environment_params)
    respond_to do |format|
      if @supported_environment
        format.html { redirect_to supported_environment_path(@supported_environment), notice: 'Supported environment was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @supported_environment.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /supported_environments/1
  # DELETE /supported_environments/1.json
  def destroy
    SupportedEnvironment.delete_force_supported_environment(params[:id])
    respond_to do |format|
      format.html { redirect_to supported_environments_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_supported_environment
      @supported_environment = SupportedEnvironment.get_force_supported_environment_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def supported_environment_params
      params.require(:supported_environment).permit(:name, :description, :config, :environment_provider_id)
    end
end
