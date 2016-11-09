class ProductSupportedEnvironmentMapsController < ApplicationController
  before_action :set_product_supported_environment_map, only: [:show, :edit, :update, :destroy]

  # GET /product_supported_environment_maps
  # GET /product_supported_environment_maps.json
  def index
    @product_supported_environment_maps = ProductSupportedEnvironmentMap.get_all_force_supported_environment_product_maps()
  end

  # GET /product_supported_environment_maps/1
  # GET /product_supported_environment_maps/1.json
  def show
  end

  # GET /product_supported_environment_maps/new
  def new
    @product_supported_environment_map = ProductSupportedEnvironmentMap.new
  end

  # GET /product_supported_environment_maps/1/edit
  def edit
    def @product_supported_environment_map.new_record?()
      false
    end
  end

  # POST /product_supported_environment_maps
  # POST /product_supported_environment_maps.json
  def create
    @product_supported_environment_map = ProductSupportedEnvironmentMap.create_force_supported_environment_product_map(product_supported_environment_map_params)

    respond_to do |format|
      if @product_supported_environment_map
        format.html { redirect_to product_supported_environment_map_path(@product_supported_environment_map), notice: 'Product supported environment map was successfully created.' }
        format.json { render action: 'show', status: :created, location: @product_supported_environment_map }
      else
        format.html { render action: 'new' }
        format.json { render json: @product_supported_environment_map.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /product_supported_environment_maps/1
  # PATCH/PUT /product_supported_environment_maps/1.json
  def update
    @product_supported_environment_map = ProductSupportedEnvironmentMap.update_force_supported_environment_product_map(params[:id] , product_supported_environment_map_params)
    respond_to do |format|
      if @product_supported_environment_map
        format.html { redirect_to product_supported_environment_map_path(@product_supported_environment_map), notice: 'Product supported environment map was successfully updated.' }
        format.json { head :no_content }
      else
        format.html { render action: 'edit' }
        format.json { render json: @product_supported_environment_map.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /product_supported_environment_maps/1
  # DELETE /product_supported_environment_maps/1.json
  def destroy
    ProductSupportedEnvironmentMap.delete_force_supported_environment_product_map(params[:id])
    respond_to do |format|
      format.html { redirect_to product_supported_environment_maps_url }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_product_supported_environment_map
      @product_supported_environment_map = ProductSupportedEnvironmentMap.get_force_supported_environment_product_map_by_id(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def product_supported_environment_map_params
      params.require(:product_supported_environment_map).permit(:project_id, :supported_environment_id)
    end
end
